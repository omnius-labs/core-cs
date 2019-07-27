using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base.Extensions
{
    public static class TaskCompletionSourceExtensions
    {
        public static async Task<TResult> WaitAsync<TResult>(this TaskCompletionSource<TResult> taskCompletionSource, CancellationToken token = default)
        {
            using (token.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                return await taskCompletionSource.Task;
            }
        }

        public static TResult Wait<TResult>(this TaskCompletionSource<TResult> taskCompletionSource, CancellationToken token = default)
        {
            using (token.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                return taskCompletionSource.Task.Result;
            }
        }
    }
}
