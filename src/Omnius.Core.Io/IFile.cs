using System.Collections.Generic;

namespace Omnius.Core.Io
{
    public interface IFile
    {
        IEnumerable<string> Glob(string glob);
    }
}
