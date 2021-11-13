using Omnius.Core.Storages.Tests.Internal;
using Omnius.Core.UnitTestToolkit;
using Xunit;

namespace Omnius.Core.Storages;

public class SingleValueFileStorageTest
{
    [Fact]
    public async Task SimpleTest()
    {
        using var deleter = FixtureFactory.GenTempDirectory(out var tempDirectoryPath);
        using var storage = SingleValueFileStorage.Factory.Create(Path.Combine(tempDirectoryPath, "test.db"), BytesPool.Shared);

        var m1 = new TestMessage("aaa");
        await storage.TrySetValueAsync(m1);

        var m2 = await storage.TryGetValueAsync<TestMessage>();

        Assert.Equal(m1, m2);
    }
}
