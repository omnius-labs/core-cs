using Core.Base;

namespace Core.Net.Connections;

public interface IBandwidthLimiter : ISynchronized
{
    int ComputeFreeBytes();

    void AddConsumedBytes(int size);
}
