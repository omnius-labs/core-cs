namespace Omnius.Core.Base.Helpers;

public static class DirectoryHelper
{
    public static bool CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return false;

        Directory.CreateDirectory(path);
        return true;
    }
}
