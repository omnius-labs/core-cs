using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnius.Core.Io
{
    public partial class FileService : IFileService
    {
        public IEnumerable<string> Glob(string glob)
        {
            throw new NotImplementedException();
        }
    }
}
