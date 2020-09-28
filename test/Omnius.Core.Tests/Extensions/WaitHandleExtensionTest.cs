using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnius.Core.Extensions
{
    public class WaitHandleExtensionTest
    {
        [Fact]
        public void WaitOneTest()
        {
            using (var e1 = new ManualResetEvent(false))
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Assert.False(e1.WaitOne(1000, cancellationTokenSource.Token));

                Task.Run(() => { cancellationTokenSource.Cancel(); });

                Assert.Throws<OperationCanceledException>(() =>
                {
                    e1.WaitOne(3000, cancellationTokenSource.Token);
                });
            }
        }
    }
}
