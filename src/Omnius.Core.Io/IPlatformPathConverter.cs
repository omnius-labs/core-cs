using System.Diagnostics.CodeAnalysis;

namespace Omnius.Core.Io
{
    public interface IPlatformPathConverter
    {
        bool TryEncoding(string platformPath, [NotNullWhen(true)] out string path);
        bool TryDecoding(string path, [NotNullWhen(true)] out string? platformPath);
    }
}
