using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Tasks
{
    public interface IBatchAction
    {
        ValueTask WaitAsync(CancellationToken cancellationToken = default);

        void Run();
    }
}
