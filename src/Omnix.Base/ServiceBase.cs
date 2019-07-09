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
        private readonly AsyncLock _asyncLock = new AsyncLock();
        public virtual ServiceStateType StateType { get; protected set; }

        protected abstract ValueTask OnStart();
        protected abstract ValueTask OnStop();

        public virtual async ValueTask StartAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Stopped)
                {
                    return;
                }

                await this.OnStart();
            }
        }

        public virtual async ValueTask StopAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Running)
                {
                    return;
                }

                await this.OnStop();
            }
        }

        public virtual async ValueTask RestartAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Stopped)
                {
                    await this.OnStop();
                }

                await this.OnStart();
            }
        }
    }
}
