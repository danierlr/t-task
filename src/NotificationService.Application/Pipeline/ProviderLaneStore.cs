using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Pipeline;

public class ProviderLaneStore {
    private readonly IReadOnlyList<ProviderLane> _allLanes;

    private Dictionary<DeliveryChannel, IReadOnlyList<ProviderLane>> _lanesByChannel = new();

    public ProviderLaneStore(IEnumerable<ProviderLane> lanes, Configuration initialConfiguration) {
        _allLanes = lanes.ToList();
        ApplyConfiguration(initialConfiguration);
    }

    public IReadOnlyList<ProviderLane> Lanes => _allLanes;

    public IReadOnlyList<ProviderLane> FindLanesByChannel(DeliveryChannel channel) {
        bool hasList = _lanesByChannel.TryGetValue(channel, out var list);

        if (!hasList) {
            throw new InvalidOperationException("Delivery lane list not found");
        }

        return list;
    }

    public void ApplyConfiguration(Configuration configuration){
        throw new NotImplementedException();
    }
}
