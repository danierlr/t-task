using NotificationService.Application.Pipeline;

namespace NotificationService.Application.Settings;

public class SettingsService {
    private PipelineSettings _settings;

    private readonly NotificationPipeline _notificationPipeline;
    public SettingsService(PipelineSettings initialSettings, NotificationPipeline notificationPipeline) {
        _settings = initialSettings;
        _notificationPipeline = notificationPipeline;
    }

    public PipelineSettings Settings => _settings;

    public void ApplySettings(PipelineSettings newSettings) {
        _notificationPipeline.ApplySettings(newSettings);
        // TODO others
    }
}
