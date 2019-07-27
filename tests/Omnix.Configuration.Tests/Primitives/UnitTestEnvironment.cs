using System.IO;

namespace Omnix.Configuration.Primitives
{
    internal static class UnitTestEnvironment
    {
        public static string TempDirectoryPath => Path.GetFullPath("Temp");
    }
}
