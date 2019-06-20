using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Omnix.Configuration.Primitives
{
    public class TestsBase : IDisposable
    {
        public TestsBase()
        {
            if (Directory.Exists(UnitTestEnvironment.TempDirectoryPath))
            {
                Directory.Delete(UnitTestEnvironment.TempDirectoryPath, true);
            }

            Directory.CreateDirectory(UnitTestEnvironment.TempDirectoryPath);
        }

        public void Dispose()
        {
            Directory.Delete(UnitTestEnvironment.TempDirectoryPath, true);
        }
    }
}
