using NotificationService.Domain.Ports;
using IdGen;
using NotificationService.Application.Shared;

namespace NotificationService.Application.IdGenerator;

// multiple generators to avoid lock contention on a single generator
// TODO ensure no id generators keys collide due to limited number of possible keys

public class SequentialIdGenerator : IIdGenerator, IDisposable {
    private static int _nextGeneratorKey = 0;


    private static readonly ThreadLocal<IdGen.IdGenerator> _generator =
        new(() => {
            int key = Interlocked.Increment(ref SequentialIdGenerator._nextGeneratorKey);
            return new IdGen.IdGenerator(key);
        });

    public string Generate() {
        long id = _generator.Value!.CreateId();

        return Base36Converter.ToBase36(id);
    }

    public void Dispose() {
        _generator.Dispose();
    }
}
