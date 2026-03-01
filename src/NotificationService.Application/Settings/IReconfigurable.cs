namespace NotificationService.Application.Settings;

public interface IReconfigurable {
    void ApplySettings(PipelineSettings settings);
}
