namespace Omnix.DataStructures
{
    public interface IVolatileCollection
    {
        /// <summary>
        /// 古いアイテムを削除します。
        /// </summary>
        void Refresh();
    }
}
