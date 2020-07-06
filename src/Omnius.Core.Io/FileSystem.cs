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
    public partial class FileSystem : IFileSystem
    {
        public IPath Path { get; } = new Path();
    }
}
