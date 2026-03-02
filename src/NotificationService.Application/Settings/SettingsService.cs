using NotificationService.Application.Notifications;
using NotificationService.Application.Pipeline;

namespace NotificationService.Application.Settings;

public class SettingsService {
    private PipelineSettings _settings;

    private readonly NotificationPipeline _notificationPipeline;
    private readonly NotificationSender _notificationSender;
    public SettingsService(
        PipelineSettings initialSettings,
        NotificationPipeline notificationPipeline,
        NotificationSender notificationSender
    ){
        _settings = initialSettings;
        _notificationPipeline = notificationPipeline;
        _notificationSender = notificationSender;
    }

    public PipelineSettings Settings => _settings;

    // TODO add api control
    public void ApplySettings(PipelineSettings newSettings) {
        _notificationSender.ApplySettings(newSettings);

        // TODO stop-the-world
        _notificationPipeline.ApplySettings(newSettings);

        // TODO others
    }
}
