using System.Collections.Generic;

namespace Omnius.Core.Io
{
    public interface IPath
    {
        IPlatformPathConverter WindowsPathConverter { get; }
    }
}
