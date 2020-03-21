using System;

namespace Omnius.Core.Collections
{
    public interface IVolatileCollection<T>
    {
        /// <summary>
        /// 古いアイテムを削除します。
        /// </summary>
        void Refresh();

        public TimeSpan GetElapsedTime(T item);

        public TimeSpan SurvivalTime { get; }
    }
}
