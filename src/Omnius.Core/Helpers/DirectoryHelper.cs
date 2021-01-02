using System;
using System.IO;

namespace Omnius.Core.Helpers
{
    public static class DirectoryHelper
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
