namespace NotificationService.Application.Pipeline;

public static class RetryPolicy {
    public static DateTime FindRetryTime(int retryCount, DateTime now, double backoffMultiplier, TimeSpan initialDelay, TimeSpan maxDelay) {
        TimeSpan delay = initialDelay * Math.Pow(backoffMultiplier, retryCount);
        delay = delay < maxDelay ? delay : maxDelay;
        return now + delay;
    }
}
