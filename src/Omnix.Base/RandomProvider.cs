using System;
using System.Security.Cryptography;
using System.Threading;

namespace Omnix.Base
{
    // http://neue.cc/2013/03/06_399.html

    /// <summary>
    /// スレッドセーフな<see cref="Random"/>クラスを生成します。
    /// </summary>
    public static class RandomProvider
    {
        private static readonly ThreadLocal<Random> _randomWrapper = new ThreadLocal<Random>(() =>
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[sizeof(int)];
                rng.GetBytes(buffer);
                return new Random(BitConverter.ToInt32(buffer, 0));
            }
        });

        public static Random GetThreadRandom()
        {
            return _randomWrapper.Value;
        }
    }
}
