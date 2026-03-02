# Notification Service

A high-throughput notification delivery service built with .NET 10 that supports multiple delivery channels (SMS, Email, Push) with automatic provider failover, retry with exponential backoff, and runtime configuration updates.

### Architecture

The service follows a layered architecture with Domain-Driven Design principles.

Domain layer contains the core business logic: the Notification aggregate with status transitions (Processing → Sent/Failed), typed recipients (SmsRecipient, EmailRecipient, PushRecipient), domain events, and provider port interfaces. The domain has no external dependencies.

Application layer implements the notification processing pipeline. The central NotificationPipeline runs on a dedicated high-priority thread and coordinates all processing through a single-threaded worker loop that handles three phases each iteration: expiring timed-out notifications, assigning retry-ready notifications to provider lanes, and processing newly submitted notifications. Provider lanes manage concurrent delivery to external providers with per-lane buffer capacity and configurable concurrency slots. The NotificationSettleRegistry enables synchronous-feeling API responses by allowing HTTP request threads to wait (with configurable linger duration) for pipeline settlement before returning.

Infrastructure layer provides concrete implementations: fake notification providers with configurable success rates and latency distributions, in-memory retry queues backed by removable priority queues, configuration binding from appsettings.json, and the hosted service that manages the pipeline worker thread lifecycle.

API layer exposes HTTP endpoints for sending notifications and managing configuration.

### Pipeline Design

The pipeline processes notifications through a series of stages, each tracked by a ProcessingState:

The notification enters the pipeline via an unbounded Channel and is assigned QueuedForProcessing state. The pipeline worker attempts to assign it to an available provider lane based on channel and provider priority. If all lanes for that channel are at capacity, the entry goes to a per-channel retry queue with QueuedForCapacityRetry state and a retry time of "now" (meaning it will be picked up as soon as a slot opens). Once assigned to a lane, the entry transitions to QueuedForProvider, then to Sending when a lane worker picks it up and begins the external provider call.

If the provider call succeeds, the notification is settled as Sent. If it fails, the pipeline calculates the next retry time using exponential backoff (initialDelay * backoffMultiplier ^ retryCount, capped at maxRetryDelay) and places the entry back in the retry queue with QueuedForProviderRetry state. If the calculated retry time exceeds the notification's TTL, or the maximum retry count is reached, the notification is settled as Failed.

Each delivery channel has its own retry queue to prevent one slow channel from delaying others - a slow SMS provider doesn't delay email delivery.

### Provider Lanes

Each provider lane manages a buffer of pending notifications and a pool of concurrent workers. Workers are spawned on-demand: when an entry is submitted, the lane checks if active workers are below the concurrency slot limit and spawns a new one if possible. Workers drain the lane's inbound channel, process entries, and exit when the channel is empty. This avoids idle long-running tasks while ensuring immediate processing of new entries.

A race condition is handled at worker exit: after releasing the concurrency slot, the worker peeks the channel - if an item was written between the last TryRead (which returned false) and the slot release, a new worker is spawned to handle it.
Each provider call is bounded by a configurable send timeout using a linked CancellationTokenSource that combines the global shutdown token with a per-request deadline.

### Concurrency Model

The pipeline worker runs on a single dedicated thread (ThreadPriority.AboveNormal) using AutoResetEvent for efficient sleep/wake coordination. The worker sleeps when there's no immediate work and calculates the optimal sleep duration based on the earliest upcoming retry or expiry time across all queues. It wakes when new notifications are submitted, provider lanes complete sends, or retry entries are enqueued.

Per-notification locking uses a RAII pattern via ILockable / LockToken - a disposable struct that acquires Monitor.Enter on construction and Monitor.Exit on dispose. This protects concurrent mutations from the pipeline worker and lane workers (e.g., a timeout expiring a notification while a provider call completes).
Capacity is tracked at two levels: a global CapacityLimiter on the pipeline controls total in-flight notifications, and per-lane capacity limiters control buffer sizes and concurrency slots. All use lock-free Interlocked operations.

The design avoids contention bottlenecks. The pipeline worker is single-threaded, so retry queues and channel state are accessed without locks. Per-notification locks are only contested between the pipeline worker and the specific lane worker processing that notification - never more than two threads. Per-lane capacity limiters use lock-free Interlocked operations. There is no shared lock that multiple threads compete for simultaneously.

### Configuration

Settings are split into two layers. Infrastructure Configuration classes bind directly to appsettings.json using raw primitives (milliseconds as long). Application Settings records use typed values (TimeSpan, domain value objects) and are immutable. A SettingsMapper converts between the two at startup. Runtime configuration updates are pushed to all IReconfigurable components.
appsettings.json structure

```
{
  "Pipeline": {
    "TotalNotificationCapacity": 1000,
    "ConfigurationUpdateTimeoutMs": 5000,
    "Channels": {
      "Sms": {
        "MaxNumRetries": 5,
        "ResultLingerDurationMs": 2000,
        "TimeToLiveMs": 2400000,
        "InitialRetryDelayMs": 5000,
        "BackoffMultiplier": 1.8,
        "MaxRetryDelayMs": 420000,
        "Providers": {
          "twilio": {
            "Enabled": true,
            "Priority": 100,
            "NumConcurrencySlots": 8,
            "BufferCapacity": 100,
            "SendTimeoutMs": 60000
          }
        }
      }
    }
  }
}
```

### Design Decisions

Single-threaded pipeline worker over a fully async pipeline - simplifies reasoning about state, avoids lock contention on retry queues, and allows the worker to make globally-optimal scheduling decisions (e.g., skipping full channels to avoid busy-waiting).

Per-channel retry queues over a single shared queue - prevents one slow channel from delaying others. A struggling SMS provider doesn't delay email delivery.
Disposable lock tokens (RAII) over manual lock/unlock - using var _ = entry.Lock() guarantees unlock even on exceptions, compiles to try/finally, and the struct avoids heap allocation.

Channel<T> for inbound notifications over ConcurrentQueue<T> - provides built-in signaling that integrates with the AutoResetEvent wake pattern without additional synchronization.

On-demand worker spawning in lanes over persistent worker loops - no idle tasks consuming thread pool threads. Workers self-terminate when the buffer is empty and respawn when new work arrives.

Capacity retry with zero delay over artificial backoff - when all lanes are full, the entry gets RetryAt = now. Older entries in the retry queue have earlier timestamps, so natural priority ordering prevents starvation without needing separate delay configuration.

### Pending work

- Runtime configuration update API endpoint

- Stop-the-world settings application (pause before applying settings to certain parts)

- Graceful shutdown with drain period for in-flight provider calls

- Notification status query endpoint

- Structured logging

- Health checks and metrics

- Callback URL support for async notification status updates

- Loading notifications from the db, that have not been settled yet and persistence to actual db

- circuit breaker pattern for the external providers. Consider Polly, Microsoft.Extensions.Resilience

- Use time buckets instead of exact timestamps for retry scheduling (reduces priority queue churn and simplifies sleep duration calculation)
