using System.Threading.Tasks;

namespace Omnix.Base
{
    public interface IService
    {
        ServiceStateType StateType { get; }

        ValueTask RestartAsync();
        ValueTask StartAsync();
        ValueTask StopAsync();
    }
}