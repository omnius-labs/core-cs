using System.IO;
using System.Threading.Tasks;
using Omnius.Core.Storages.Tests.Internal;
using Omnius.Core.UnitTestToolkit;
using Xunit;

namespace Omnius.Core.Storages
{
    public class KeyValueLiteDatabaseStorageTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            using var deleter = FixtureFactory.GenTempDirectory(out var tempDirectoryPath);
            using var storage = KeyValueLiteDatabaseStorage.Factory.Create<string>(Path.Combine(tempDirectoryPath, "test.db"), BytesPool.Shared);
            await storage.MigrateAsync();
            await storage.RebuildAsync();

            var m1 = new TestMessage("aaa");
            await storage.TrySetValueAsync("a", m1);

            var m2 = await storage.TryGetValueAsync<TestMessage>("a");

            Assert.Equal(m1, m2);

            await storage.TryChangeKeyAsync("a", "b");

            var m3 = await storage.TryGetValueAsync<TestMessage>("b");

            Assert.Equal(m1, m3);
        }
    }
}
