using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omnix.Base.Extensions
{
    public static class ValueTaskHelper
    {
        public static async Task WhenAll(params ValueTask[] tasks)
        {
            foreach (var valueTask in tasks)
            {
                if (!valueTask.IsCompletedSuccessfully)
                {
                    await valueTask;
                }
            }
        }

        public static async Task<T[]> WhenAll<T>(params ValueTask<T>[] tasks)
        {
            var results = new List<T>();

            foreach (var valueTask in tasks)
            {
                if (valueTask.IsCompletedSuccessfully)
                {
                    results.Add(valueTask.Result);
                }
                else
                {
                    results.Add(await valueTask);
                }
            }

            return results.ToArray();
        }
    }
}
