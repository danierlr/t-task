using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Aggregates.Notification;

public abstract record Recipient { }

public sealed record SmsRecipient : Recipient {
    public string Phone { get; }

    public SmsRecipient(string phone) {
        if (string.IsNullOrWhiteSpace(phone)) {
            throw new ArgumentException("Phone must not be empty", nameof(phone));
        }

        Phone = phone;
    }
}

public sealed record EmailRecipient : Recipient {
    public string Email { get; }

    public EmailRecipient(string email) {
        // TODO better email validation

        if (string.IsNullOrWhiteSpace(email)) {
            throw new ArgumentException("Email must not be empty", nameof(email));
        }

        Email = email;
    }
}

public sealed record PushRecipient : Recipient {
    public string DeviceToken { get; }

    // TODO add all stuff needed for push

    public PushRecipient(string deviceToken) {
        if (string.IsNullOrWhiteSpace(deviceToken)) {
            throw new ArgumentException("Device token must not be empty", nameof(deviceToken));
        }

        DeviceToken = deviceToken;
    }
}