using System.Threading.Tasks;

namespace Omnix.Base
{
    public interface IService
    {
        ServiceStateType StateType { get; }

        ValueTask Restart();
        ValueTask Start();
        ValueTask Stop();
    }
}