using System.Collections.Generic;

namespace Omnius.Core.Io
{
    public interface IFileService
    {
        IEnumerable<string> Glob(string glob);
    }
}
