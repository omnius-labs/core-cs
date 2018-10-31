using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    public enum ServiceState
    {
        Starting,
        Running,
        Stopping,
        Stopped,
    }

    /// <summary>
    /// Statefulなクラスの実行状態を管理します。
    /// </summary>
    public abstract class ServiceBase : DisposableBase
    {
        public abstract ValueTask Start(CancellationToken token = default);
        public abstract ValueTask Stop(CancellationToken token = default);
        public abstract ValueTask Restart(CancellationToken token = default);

        public abstract ServiceState State { get; }
    }
}
