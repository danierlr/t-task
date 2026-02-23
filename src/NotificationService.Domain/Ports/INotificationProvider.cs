using System;
using System.Collections.Generic;
using System.Text;
using NotificationService.Domain.Aggregates.Notification;

namespace NotificationService.Domain.Ports;

public interface INotificationProvider {
    string Name { get; }

    Channel Channel { get; }

    Task<bool> TrySendAsync(Recipient recipient, string message, CancellationToken cancellationToken);
}
