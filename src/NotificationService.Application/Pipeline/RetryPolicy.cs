namespace NotificationService.Application.Pipeline;

public static class RetryPolicy {
    public static DateTime FindRetryTime(long retryCount, DateTime now, double backoffMultiplier, TimeSpan initialDelay, TimeSpan maxDelay) {
        TimeSpan delay = initialDelay * Math.Pow(backoffMultiplier, retryCount);
        delay = delay < maxDelay ? delay : maxDelay;
        return now + delay;
    }
}
