using Omnius.Core.Storages.Tests.Internal;
using Omnius.Core.UnitTestToolkit;
using Xunit;

namespace Omnius.Core.Storages;

public class KeyValueStorageTest
{
    [Fact]
    public async Task SimpleTest()
    {
        await using var container = new StorageContainer(KeyValueFileStorage.Factory);
        await container.Storage.MigrateAsync();

        await NewMethod(storage);
    }

    private class StorageContainer : IAsyncDisposable
    {
        private readonly IDisposable _deleter;

        public StorageContainer(IKeyValueStorageFactory factory)
        {
            _deleter = FixtureFactory.GenTempDirectory(out var tempDirectoryPath);
            this.Storage = factory.Create(tempDirectoryPath, BytesPool.Shared);
        }

        public IKeyValueStorage Storage { get; }

        public async ValueTask DisposeAsync()
        {
            await this.Storage.DisposeAsync();
            _deleter.Dispose();
        }
    }

    // ValueTask MigrateAsync(CancellationToken cancellationToken = default);
    // ValueTask RebuildAsync(CancellationToken cancellationToken = default);
    // ValueTask<bool> TryChangeKeyAsync(string oldKey, string newKey, CancellationToken cancellationToken = default);
    // ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default);
    // IAsyncEnumerable<string> GetKeysAsync(CancellationToken cancellationToken = default);
    // ValueTask<IMemoryOwner<byte>?> TryReadAsync(string key, CancellationToken cancellationToken = default);
    // ValueTask<bool> TryReadAsync(string key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default);
    // ValueTask WriteAsync(string key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default);
    // ValueTask WriteAsync(string key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);
    // ValueTask<bool> TryDeleteAsync(string key, CancellationToken cancellationToken = default);
    // ValueTask ShrinkAsync(IEnumerable<string> excludedKeys, CancellationToken cancellationToken = default);

    private static async Task TryChangeKeyTest(IKeyValueStorage storage)
    {
        {
            var m = new TestMessage("aaa");
            await storage.WriteAsync("a", m);
        }

        var m2 = await storage.TryReadAsync<TestMessage>("a");

        Assert.Equal(m1, m2);

        await storage.TryChangeKeyAsync("a", "b");

        var m3 = await storage.TryReadAsync<TestMessage>("b");

        Assert.Equal(m1, m3);
    }

    [Fact]
    public async Task KeyValueFileStorage_ComputeRelativeFilePathTest()
    {
        Assert.Equal("0/0/0/0/0/0", KeyValueFileStorage.ComputeRelativeFilePath(0));
        Assert.Equal("0/0/0/0/0/1", KeyValueFileStorage.ComputeRelativeFilePath(1));
        Assert.Equal("0/0/0/0/0/2047", KeyValueFileStorage.ComputeRelativeFilePath(0x7FF));
        Assert.Equal("0/0/0/0/1/0", KeyValueFileStorage.ComputeRelativeFilePath(0x7FF + 1));
        Assert.Equal("0/0/0/0/2047/2047", KeyValueFileStorage.ComputeRelativeFilePath(0x3FFFFF));
        Assert.Equal("0/0/0/1/0/0", KeyValueFileStorage.ComputeRelativeFilePath(0x3FFFFF + 1));
        Assert.Equal("255/2047/2047/2047/2047/2047", KeyValueFileStorage.ComputeRelativeFilePath(long.MaxValue));
    }
}
