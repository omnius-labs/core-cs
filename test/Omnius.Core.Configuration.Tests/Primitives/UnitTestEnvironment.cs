using System.IO;

namespace Omnius.Core.Configuration.Primitives
{
    internal static class UnitTestEnvironment
    {
        public static string TempDirectoryPath => Path.GetFullPath("Temp");
    }
}
