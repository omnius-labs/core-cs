using System;
using Omnius.Core.Configuration.Primitives;
using Xunit;

namespace Omnius.Core.Configuration
{
    public class SettingsDatabaseTests : TestsBase
    {
        [Fact]
        public void VersionReadWriteTest()
        {
            using var settings = new OmniSettings(UnitTestEnvironment.TempDirectoryPath);

            var random = new Random();

            for (int i = 0; i < 32; i++)
            {
                uint setVersion = (uint)random.Next();

                settings.SetVersion(setVersion);
                Assert.True(settings.TryGetVersion(out var getVersion));
                Assert.True(setVersion == getVersion);
            }
        }
    }
}
