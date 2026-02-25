using NotificationService.Api.Shared;
using NotificationService.Application.Notifications.Dtos;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Api.Notifications;

public static class NotificationMappingExtensions {
    public static SendNotificationRequest ToSendNotificationRequest(this SendNotificationHttpRequest httpRequest) {
        Channel channel = EnumParser.ParseFromSnakeCase<Channel>(httpRequest.Channel);

        string GetRecipientProperty(string key) {
            string? value;

            httpRequest.Recipient.TryGetValue(key, out value);

            if (string.IsNullOrWhiteSpace(value)){
                throw new ArgumentException($"Missing required recipient field: '{key}'");
            }

            return value;
        }

        Recipient recipient = channel switch {
            Channel.Sms => new SmsRecipient(GetRecipientProperty("phone")),
            Channel.Email => new EmailRecipient(GetRecipientProperty("email")),
            Channel.Push => new PushRecipient(GetRecipientProperty("device_token")),
            _ => throw new ArgumentException($"Channel is not supported: {channel}")
        };

        return new SendNotificationRequest(
            Recipient: recipient,
            Message: httpRequest.Message,
            Channel: channel
        );
    }
}
