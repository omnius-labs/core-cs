namespace Omnius.Core.Tasks;

public interface IBatchAction
{
    TimeSpan Interval { get; }

    void Execute();
}
