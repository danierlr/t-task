using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;

namespace NotificationService.Infrastructure.Providers.Fake;

public class FakeProvider<TRecipient> : NotificationProvider<TRecipient> where TRecipient : Recipient {
    private readonly string _name;
    private readonly DeliveryChannel _channel;
    private readonly double _successChance;
    private readonly double _delayAverageMs;
    private readonly double _delayStdDeviationMs;

    public FakeProvider(string name, DeliveryChannel channel, double successChance, double delayAverageMs, double delayStdDeviationMs) {
        _name = name;
        _channel = channel;
        _successChance = successChance;
        _delayAverageMs = delayAverageMs;
        _delayStdDeviationMs = delayStdDeviationMs;
    }

    public override string Name => _name;

    public override DeliveryChannel Channel => _channel;

    protected override async Task<bool> TryInternalSendAsync(TRecipient recipient, string message, CancellationToken cancellationToken) {
        var roll = Random.Shared.NextDouble();
        int delayMs = Math.Max(0, (int)Math.Round(FindGaussian(_delayAverageMs, _delayStdDeviationMs)));

        await Task.Delay(delayMs, cancellationToken);

        return roll <= _successChance;
    }

    // find random value from normal distribution parameters
    private double FindGaussian(double mean, double stdDev) {
        var u1 = 1.0 - Random.Shared.NextDouble();
        var u2 = Random.Shared.NextDouble();
        var normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * normal;
    }
}
