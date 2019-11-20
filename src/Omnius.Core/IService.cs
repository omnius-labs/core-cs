using System.Threading.Tasks;

namespace Omnius.Core
{
    public interface IService
    {
        ServiceStateType StateType { get; }

        ValueTask RestartAsync();
        ValueTask StartAsync();
        ValueTask StopAsync();
    }
}
