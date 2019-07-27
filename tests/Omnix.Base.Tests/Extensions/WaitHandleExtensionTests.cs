using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Base.Extensions
{
    public class WaitHandleExtensionTests
    {
        [Fact]
        public void WaitOneTest()
        {
            using (var e1 = new ManualResetEvent(false))
            using (var tokenSource = new CancellationTokenSource())
            {
                Assert.False(e1.WaitOne(1000, tokenSource.Token));

                Task.Run(() => { tokenSource.Cancel(); });

                Assert.Throws<OperationCanceledException>(() =>
                {
                    e1.WaitOne(3000, tokenSource.Token);
                });
            }
        }
    }
}
