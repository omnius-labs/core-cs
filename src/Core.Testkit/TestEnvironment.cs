namespace Omnius.Core.Testkit;

public class TestEnvironment
{
    public static string GetBasePath()
    {
        var basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly()?.Location);
        basePath ??= Directory.GetCurrentDirectory();
        return basePath;
    }
}
