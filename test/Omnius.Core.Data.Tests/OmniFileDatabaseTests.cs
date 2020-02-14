using System;
using System.Threading.Tasks;
using Omnius.Core.Configuration.Primitives;
using Omnius.Core.Data;
using Omnius.Core.Data.Tests.Internal;
using Xunit;

namespace Omnius.Core.Data
{
    public class OmniFileDatabaseTests : TestsBase
    {
        [Fact]
        public async ValueTask VersionReadWriteTest()
        {
            await using var settings = new OmniFileDatabase(UnitTestEnvironment.TempDirectoryPath);

            var random = new Random();

            for (int i = 0; i < 32; i++)
            {
                var value = new TestObject(random.Next());

                await settings.SaveAsync("version", value);
                Assert.Equal(value, await settings.LoadAsync<TestObject>("version"));
            }
        }
    }
}
