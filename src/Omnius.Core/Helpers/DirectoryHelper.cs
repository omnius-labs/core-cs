using System;
using System.IO;

namespace Omnius.Core.Helpers
{
    public static class DirectoryHelper
    {
        public static bool CreateDirectory(string path)
        {
            if (Directory.Exists(path)) return false;

            Directory.CreateDirectory(path);
            return true;
        }
    }
}
