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
            using (var manualResetEvent = new ManualResetEvent(false))
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Assert.False(manualResetEvent.WaitOne(1000, cancellationTokenSource.Token));

                Task.Run(() => { cancellationTokenSource.Cancel(); });

                Assert.Throws<OperationCanceledException>(() =>
                {
                    manualResetEvent.WaitOne(3000, cancellationTokenSource.Token);
                });
            }
        }
    }
}
