using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Omnius.Core.Io
{
    public sealed class WindowsPathConverter : IPlatformPathConverter
    {
        public bool TryEncoding(string platformPath, [NotNullWhen(true)] out string? path)
        {
            path = null;

            if (platformPath.Length < 3)
            {
                return false;
            }

            if (!(('a' <= platformPath[0] && platformPath[0] <= 'z') || ('A' <= platformPath[0] && platformPath[0] <= 'Z')))
            {
                return false;
            }

            if (!(platformPath[1] == ':' && platformPath[2] == '\\'))
            {
                return false;
            }

            var sb = new StringBuilder();
            sb.Append("/" + platformPath[0] + "/");
            sb.Append(platformPath.Substring(3).Replace('\\', '/'));

            path = sb.ToString();
            return true;
        }

        public bool TryDecoding(string platformPath, [NotNullWhen(true)] out string? path)
        {
            path = null;

            var sections = platformPath.Split('\\');

            // フォーマットのチェック
            if (sections.Length < 1 || !(sections[0].Length == 1))
            {
                return false;
            }

            var driveName = sections[0][0];

            if (!(('a' <= driveName && driveName <= 'z') || ('A' <= driveName && driveName <= 'Z')))
            {
                return false;
            }

            var sb = new StringBuilder();
            sb.Append(driveName + @":\");
            sb.Append(string.Join('\\', sections.Skip(1)));

            path = sb.ToString();
            return true;
        }
    }
}
