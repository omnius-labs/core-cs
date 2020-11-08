using System.IO;

namespace Omnius.Core.UnitTestToolkit
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
