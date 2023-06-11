using Omnius.Core.Storages.Tests.Internal;
using Omnius.Core.UnitTestToolkit;
using Xunit;

namespace Omnius.Core.Storages;

public class SingleValueStorageTest
{
    [Fact]
    public async Task SingleValueFileStorageTest()
    {
        {
            await using var container = new StorageContainer(SingleValueFileStorage.Factory);
            await this.SetValueAndTryGetValueTest(container.Storage);
        }

        {
            await using var container = new StorageContainer(SingleValueFileStorage.Factory);
            await this.SetValueAndTryDeleteTest(container.Storage);
        }
    }

    private class StorageContainer : IAsyncDisposable
    {
        private readonly IDisposable _deleter;

        public StorageContainer(ISingleValueStorageFactory factory)
        {
            _deleter = FixtureFactory.GenTempDirectory(out var tempDirectoryPath);
            this.Storage = factory.Create(Path.Combine(tempDirectoryPath, "test.db"), BytesPool.Shared);
        }

        public ISingleValueStorage Storage { get; }

        public async ValueTask DisposeAsync()
        {
            await this.Storage.DisposeAsync();
            _deleter.Dispose();
        }
    }

    private async Task SetValueAndTryGetValueTest(ISingleValueStorage storage)
    {
        {
            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(res, default(TestMessage));
        }

        {
            var m = new TestMessage("aaa");
            await storage.SetValueAsync(m);

            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(m, res);
        }
    }

    private async Task SetValueAndTryDeleteTest(ISingleValueStorage storage)
    {
        {
            var res = await storage.TryDeleteAsync();
            Assert.False(res);
        }

        {
            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(res, default(TestMessage));
        }

        {
            var m = new TestMessage("aaa");
            await storage.SetValueAsync(m);

            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(m, res);
        }

        {
            var res = await storage.TryDeleteAsync();
            Assert.True(res);
        }

        {
            var res = await storage.TryDeleteAsync();
            Assert.False(res);
        }

        {
            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(res, default(TestMessage));
        }
    }
}
