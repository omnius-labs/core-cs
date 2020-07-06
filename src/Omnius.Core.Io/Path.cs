using System.Collections.Generic;

namespace Omnius.Core.Io
{
    public class Path : IPath
    {
        public IPlatformPathConverter WindowsPathConverter { get; } = new WindowsPathConverter();
    }
}
