using System;
using Omnix.Configuration.Primitives;
using Xunit;

namespace Omnix.Configuration
{
    public class SettingsDatabaseTests : TestsBase
    {
        [Fact]
        public void VersionReadWriteTest()
        {
            using var settings = new SettingsDatabase(UnitTestEnvironment.TempDirectoryPath);

            var random = new Random();

            for (int i = 0; i < 32; i++)
            {
                uint setVersion = (uint)random.Next();

                Assert.True(settings.TrySetVersion(setVersion));
                Assert.True(settings.TryGetVersion(out var getVersion));
                Assert.True(setVersion == getVersion);
            }
        }
    }
}
