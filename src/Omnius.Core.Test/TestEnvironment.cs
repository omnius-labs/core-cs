using System.IO;

namespace Omnius.Core.Test
{
    public class TestEnvironment
    {
        public static string GetBasePath()
        {
            var basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly()!.Location)!;
            return basePath;
        }
    }
}
