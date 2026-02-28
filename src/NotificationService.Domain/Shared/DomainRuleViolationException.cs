namespace NotificationService.Domain.Shared;

public class DomainRuleViolationException : Exception {
    public DomainRuleViolationException(string message) : base(message) { }
}
