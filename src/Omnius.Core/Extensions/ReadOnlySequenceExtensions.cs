using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Omnius.Core.Internal;

namespace Omnius.Core.Extensions
{
    public static class ReadOnlySequenceExtensions
    {
        public static void CopyTo(this ReadOnlySequence<byte> sequence, Stream stream)
        {
            foreach (var memory in sequence)
            {
                stream.Write(memory.Span);
            }
        }
    }
}
