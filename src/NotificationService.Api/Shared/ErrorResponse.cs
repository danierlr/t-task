using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.Shared {
    public class ErrorResponse : ProblemDetails {
        // Similar to sentry context - allows to add more error info of arbitrary shape
        public object? Context { get; set; }
    }
}
