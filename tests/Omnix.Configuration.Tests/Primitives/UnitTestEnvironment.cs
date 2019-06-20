using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Omnix.Configuration.Primitives
{
    static class UnitTestEnvironment
    {
        public static string TempDirectoryPath => Path.GetFullPath("Temp");
    }
}
