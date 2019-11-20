using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Omnius.Core.Network
{
    public partial class OmniPath
    {
        public string[] Decompose()
        {
            return this.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        public static OmniPath Combine(params OmniPath[] omniPaths)
        {
            var sb = new StringBuilder();
            sb.Append('/');

            foreach (var omniPath in omniPaths)
            {
                sb.Append(omniPath.Value.Trim('/'));
                sb.Append('/');
            }

            return new OmniPath(sb.ToString());
        }

        public static implicit operator string(OmniPath omniPath)
        {
            return omniPath.Value;
        }

        public static implicit operator OmniPath(string text)
        {
            return new OmniPath(text);
        }

        public static class Windows
        {
            public static bool TryEncoding(string path, out OmniPath? omniPath)
            {
                omniPath = null;

                if (path.Length < 3)
                {
                    return false;
                }

                if (!(('a' <= path[0] && path[0] <= 'z') || ('A' <= path[0] && path[0] <= 'Z')))
                {
                    return false;
                }

                if (!(path[1] == ':' && path[2] == '\\'))
                {
                    return false;
                }

                var sb = new StringBuilder();
                sb.Append("/" + path[0] + "/");
                sb.Append(path.Substring(3).Replace('\\', '/'));

                omniPath = new OmniPath(sb.ToString());
                return true;
            }

            public static bool TryDecoding(OmniPath omniPath, out string? path)
            {
                path = null;

                var sections = omniPath.Decompose();

                // フォーマットのチェック
                if (sections.Length < 1 || !(sections[1].Length == 1))
                {
                    return false;
                }

                var driveName = sections[1][0];

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
}
