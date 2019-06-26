using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    public enum ServiceStateType
    {
        Starting,
        Running,
        Stopping,
        Stopped,
    }

    /// <summary>
    /// Statefulなクラスの実行状態を管理します。
    /// </summary>
    public abstract class ServiceBase : DisposableBase, IService
    {
        public abstract ValueTask StartAsync();
        public abstract ValueTask StopAsync();
        public abstract ValueTask RestartAsync();

        public abstract ServiceStateType StateType { get; }
    }
}
