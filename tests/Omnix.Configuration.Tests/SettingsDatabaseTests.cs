using System;
using Omnix.Configuration.Tests.Primitives;
using Xunit;

namespace Omnix.Configuration
{
    public class SettingsDatabaseTests : TestsBase
    {
        [Fact]
        public void Test1()
        {
            using var settings = new SettingsDatabase(UnitTestEnvironment.TempDirectoryPath);
        }
    }
}
