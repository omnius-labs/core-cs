using System.Threading.Tasks;
using Omnius.Core.Storages.Tests.Internal;
using Xunit;

namespace Omnius.Core.Storages
{
    public class LiteDatabaseBytesStorageTest
    {
        [Fact]
        public async Task TestName()
        {
            var storage = LiteDatabaseBytesStorage.Factory.Create<string>("test.db", BytesPool.Shared);
            var m = new TestMessage("aaa");
            await storage.SetValueAsync("a", m);
        }
    }
}
