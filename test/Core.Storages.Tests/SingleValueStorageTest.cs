using Omnius.Core.Base;
using Omnius.Core.Storages.Tests.Internal;
using Omnius.Core.Testkit;
using Xunit;

namespace Omnius.Core.Storages;

public class SingleValueStorageTest
{
    [Fact]
    public async Task SingleValueFileStorageTest()
    {
        foreach (var factory in new[] { SingleValueFileStorage.Factory })
        {
            {
                await using var container = new StorageContainer(factory);
                await this.SetValueAndTryGetValueTestAsync(container.Storage);
            }

            {
                await using var container = new StorageContainer(factory);
                await this.SetValueAndTryDeleteTestAsync(container.Storage);
            }
        }
    }

    private class StorageContainer : IAsyncDisposable
    {
        private readonly IDisposable _disposable;

        public StorageContainer(ISingleValueStorageFactory factory)
        {
            _disposable = FixtureFactory.GenTempDirectory(out var tempDirectoryPath);
            this.Storage = factory.Create(Path.Combine(tempDirectoryPath, "test.db"), BytesPool.Shared);
        }

        public ISingleValueStorage Storage { get; }

        public async ValueTask DisposeAsync()
        {
            await this.Storage.DisposeAsync();
            _disposable.Dispose();
        }
    }

    private async Task SetValueAndTryGetValueTestAsync(ISingleValueStorage storage)
    {
        {
            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(default(TestMessage), res);
        }

        {
            var m = new TestMessage("aaa");
            await storage.SetValueAsync(m);

            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(m, res);
        }
    }

    private async Task SetValueAndTryDeleteTestAsync(ISingleValueStorage storage)
    {
        {
            var res = await storage.TryDeleteAsync();
            Assert.False(res);
        }

        {
            var res = await storage.TryGetValueAsync<TestMessage>();
            Assert.Equal(default(TestMessage), res);
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
            Assert.Equal(default(TestMessage), res);
        }
    }
}
