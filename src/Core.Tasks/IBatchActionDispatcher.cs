namespace Core.Tasks;

public interface IBatchActionDispatcher
{
    void Register(IBatchAction batchAction);
    void Unregister(IBatchAction batchAction);
}
