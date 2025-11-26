using Omnius.Core.Base;
using Omnius.Core.RocketPack;
using Omnius.Core.Testkit;
using Xunit;

namespace Omnius.Core.Storages;

public class KeyValueStorageTest
{
    [Fact]
    public async Task KeyValueStorageAllTest()
    {
        var fixtureFactory = new FixtureFactory(1);

        foreach (var factory in new[] { KeyValueFileStorage.Factory })
        {
            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.RebuildTestAsync(container.Storage);
            }

            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.TryChangeKeyAndReadWriteTestAsync(container.Storage);
            }

            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.ContainsKeyAndGetKeysTestAsync(container.Storage);
            }

            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.TryDeleteAndShrinkTestAsync(container.Storage);
            }
        }
    }

    private class StorageContainer : IAsyncDisposable
    {
        private readonly IDisposable _disposable;

        public StorageContainer(IKeyValueStorageFactory factory, FixtureFactory fixtureFactory)
        {
            _disposable = fixtureFactory.GenTempDirectory(out var tempDirectoryPath);
            this.Storage = factory.Create(tempDirectoryPath, BytesPool.Shared);
        }

        public IKeyValueStorage Storage { get; }

        public async ValueTask DisposeAsync()
        {
            await this.Storage.DisposeAsync();
            _disposable.Dispose();
        }
    }

    private async Task RebuildTestAsync(IKeyValueStorage storage)
    {
        await storage.MigrateAsync();
        await storage.RebuildAsync();
    }

    private async Task TryChangeKeyAndReadWriteTestAsync(IKeyValueStorage storage)
    {
        await storage.MigrateAsync();

        var res = await storage.TryChangeKeyAsync("a", "b");
        Assert.False(res);

        var m = new TestMessage("aaa");
        await storage.WriteAsync<TestMessage>("a", m);

        var m2 = await storage.TryReadAsync<TestMessage>("a");
        Assert.Equal(m, m2);

        var res2 = await storage.TryChangeKeyAsync("a", "b");
        Assert.True(res2);

        var m3 = await storage.TryReadAsync<TestMessage>("a");
        Assert.Null(m3);

        var m4 = await storage.TryReadAsync<TestMessage>("b");
        Assert.Equal(m, m4);
    }

    private async Task ContainsKeyAndGetKeysTestAsync(IKeyValueStorage storage)
    {
        await storage.MigrateAsync();

        var res = await storage.ContainsKeyAsync("a");
        Assert.False(res);

        var ma = new TestMessage("aaa");
        await storage.WriteAsync<TestMessage>("a", ma);

        var res2 = await storage.ContainsKeyAsync("a");
        Assert.True(res2);

        var mb = new TestMessage("bbb");
        await storage.WriteAsync<TestMessage>("b", mb);

        var resList = await storage.GetKeysAsync().ToListAsync();
        resList.Sort();
        Assert.Equal(resList, new[] { "a", "b" });
    }

    private async Task TryDeleteAndShrinkTestAsync(IKeyValueStorage storage)
    {
        await storage.MigrateAsync();

        var res = await storage.TryDeleteAsync("a");
        Assert.False(res);

        var ma = new TestMessage("aaa");
        await storage.WriteAsync<TestMessage>("a", ma);

        var res2 = await storage.ContainsKeyAsync("a");
        Assert.True(res2);

        var mb = new TestMessage("bbb");
        await storage.WriteAsync<TestMessage>("b", mb);

        var res3 = await storage.TryDeleteAsync("a");
        Assert.True(res3);

        var resList = await storage.GetKeysAsync().ToListAsync();
        resList.Sort();
        Assert.Equal(resList, new[] { "b" });

        var mc = new TestMessage("ccc");
        await storage.WriteAsync<TestMessage>("c", mc);

        var res2List = await storage.GetKeysAsync().ToListAsync();
        res2List.Sort();
        Assert.Equal(res2List, new[] { "b", "c" });

        await storage.ShrinkAsync(new[] { "b" });

        var res3List = await storage.GetKeysAsync().ToListAsync();
        res3List.Sort();
        Assert.Equal(res3List, new[] { "b" });
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

    public class TestMessage : IRocketPackStruct<TestMessage>, IEquatable<TestMessage>
    {
        public TestMessage(string value)
        {
            this.Value = value;
        }

        public string Value { get; }

        private int? _hashCode;

        public override int GetHashCode()
        {
            if (_hashCode is null)
            {
                var h = new HashCode();
                h.Add(this.Value);
                _hashCode = h.ToHashCode();
            }

            return _hashCode.Value;
        }

        public bool Equals(TestMessage? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Value == other.Value;
        }

        public static void Pack(ref RocketPackBytesEncoder encoder, in TestMessage value)
        {
            encoder.WriteString(value.Value);
        }

        public static TestMessage Unpack(ref RocketPackBytesDecoder decoder)
        {
            var value = decoder.ReadString();
            return new TestMessage(value);
        }
    }
}
