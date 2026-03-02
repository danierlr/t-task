using NotificationService.Application.Settings;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Pipeline;

public class ProviderLaneStore: IReconfigurable {
    private readonly IReadOnlyList<ProviderLane> _allLanes;

    private volatile Dictionary<DeliveryChannel, IReadOnlyList<ProviderLane>> _lanesByChannel = new();

    public ProviderLaneStore(IEnumerable<ProviderLane> lanes, PipelineSettings settings) {
        _allLanes = lanes.ToList();
        ApplySettings(settings);
    }

    public IReadOnlyList<ProviderLane> Lanes => _allLanes;

    public IReadOnlyList<ProviderLane> FindLanesByChannel(DeliveryChannel channel) {
        bool hasList = _lanesByChannel.TryGetValue(channel, out var list);

        if (!hasList || list is null) {
            throw new InvalidOperationException("Delivery lane list not found");
        }

        return list;
    }

    public void ApplySettings(PipelineSettings settings) {
        Dictionary<DeliveryChannel, List<ProviderLane>> lanesByChannel = new();

        foreach (var channel in Enum.GetValues<DeliveryChannel>()) {
            lanesByChannel[channel] = new();
        }

        foreach (var lane in _allLanes) {
            var laneSettings = settings.Lanes[(lane.Provider.Channel, lane.Provider.Name)];
            lane.ApplySettings(laneSettings);

            if(laneSettings.Provider.Enabled) {
                lanesByChannel[lane.Provider.Channel].Add(lane);
            }
        }

        var newLanesByChannel = new Dictionary<DeliveryChannel, IReadOnlyList<ProviderLane>>();

        foreach (var channel in Enum.GetValues<DeliveryChannel>()) {
            lanesByChannel[channel].Sort((first, second) => second.Settings.Provider.Priority.CompareTo(first.Settings.Provider.Priority));
            newLanesByChannel[channel] = lanesByChannel[channel];
        }

        _lanesByChannel = newLanesByChannel;
    }
}
