using Microsoft.AspNetCore.Mvc;
using NotificationService.Domain.Aggregates.Notification;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Notification; 


// TODO add batch
public sealed class SendNotificationHttpRequest {
    //[Required]
    //public Recipient Recipient;

    [Required]
    public string? Message { get; set; }

    [Required]
    public Channel? Channel { get; set; }

    public string? CallbackUrl { get; set; }
}
