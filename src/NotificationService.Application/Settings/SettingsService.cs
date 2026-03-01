namespace NotificationService.Application.Settings;

public class SettingsService {
    private PipelineSettings _settings;
    public SettingsService(PipelineSettings initialSettings) {
        _settings = initialSettings;
    }

    public PipelineSettings Settings => _settings;

    public void ApplySettings(PipelineSettings newSettings) {
        throw new NotImplementedException();
    }
}
