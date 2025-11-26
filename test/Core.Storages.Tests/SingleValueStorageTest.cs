using Omnius.Core.Base;
using Omnius.Core.RocketPack;
using Omnius.Core.Testkit;
using Xunit;

namespace Omnius.Core.Storages;

public class SingleValueStorageTest
{
    [Fact]
    public async Task SingleValueStorageAllTest()
    {
        var fixtureFactory = new FixtureFactory(2);

        foreach (var factory in new[] { SingleValueFileStorage.Factory })
        {
            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.SetValueAndTryGetValueTestAsync(container.Storage);
            }

            {
                await using var container = new StorageContainer(factory, fixtureFactory);
                await this.SetValueAndTryDeleteTestAsync(container.Storage);
            }
        }
    }

    private class StorageContainer : IAsyncDisposable
    {
        private readonly IDisposable _disposable;

        public StorageContainer(ISingleValueStorageFactory factory, FixtureFactory fixtureFactory)
        {
            _disposable = fixtureFactory.GenTempDirectory(out var tempDirectoryPath);
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
