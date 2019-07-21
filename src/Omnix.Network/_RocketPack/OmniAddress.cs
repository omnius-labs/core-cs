using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Network
{
    partial class OmniAddress
    {
        public string[] Decompose()
        {
            return this.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        public static OmniAddress Combine(params OmniAddress[] omniAddresses)
        {
            var sb = new StringBuilder();
            sb.Append('/');

            foreach (var omniAddress in omniAddresses)
            {
                sb.Append(omniAddress.Value.Trim('/'));
                sb.Append('/');
            }

            return new OmniAddress(sb.ToString());
        }

        public static implicit operator string(OmniAddress omniAddress)
        {
            return omniAddress.Value;
        }

        public static implicit operator OmniAddress(string text)
        {
            return new OmniAddress(text);
        }

        public static class IpAddress
        {
            public static class Tcp
            {
                public static bool TryEncoding(IPAddress ipAddress, ushort port, out OmniAddress? omniAddress)
                {
                    omniAddress = null;

                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        omniAddress = new OmniAddress($"/ip4/{ipAddress.Address}/tcp/{port}");
                        return true;
                    }
                    else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        omniAddress = new OmniAddress($"/ip6/{ipAddress.Address}/tcp/{port}");
                        return true;
                    }

                    return false;
                }

                public static bool TryDecoding(OmniAddress omniAddress, out IPAddress ipAddress, out ushort port, out int consumed, bool nameResolving = false)
                {
                    ipAddress = IPAddress.None;
                    port = 0;
                    consumed = 0;

                    var sections = omniAddress.Decompose();

                    // フォーマットのチェック
                    if (sections.Length < 4 || !(sections[0] == "ip4" || sections[0] == "ip6") || !(sections[2] == "tcp"))
                    {
                        return false;
                    }

                    // IPアドレスのパース処理
                    {
                        if (nameResolving)
                        {
                            if (!IPAddress.TryParse(sections[1], out ipAddress))
                            {
                                try
                                {
                                    var hostEntry = Dns.GetHostEntry(sections[1]);

                                    if (hostEntry.AddressList.Length == 0)
                                    {
                                        return false;
                                    }

                                    ipAddress = hostEntry.AddressList[0];
                                }
                                catch (Exception e)
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!IPAddress.TryParse(sections[1], out ipAddress))
                            {
                                return false;
                            }
                        }

                        if (sections[0] == "ip4" && ipAddress.AddressFamily != AddressFamily.InterNetwork)
                        {
                            return false;
                        }

                        if (sections[0] == "ip6" && ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            return false;
                        }
                    }

                    // ポート番号のパース処理
                    if (ushort.TryParse(sections[3], out port))
                    {
                        return false;
                    }

                    consumed = 4;

                    return true;
                }
            }
        }

        public static class Windows
        {
            public static class FileSystem
            {
                public static bool TryEncoding(string path, out OmniAddress? omniAddress)
                {
                    omniAddress = null;

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
                    sb.Append("/fs/" + path[0] + "/");
                    sb.Append(path.Substring(3).Replace('\\', '/'));

                    omniAddress = new OmniAddress(sb.ToString());
                    return true;
                }

                public static bool TryDecoding(OmniAddress omniAddress, out string? path, out int consumed)
                {
                    path = null;
                    consumed = 0;

                    var sections = omniAddress.Decompose();

                    // フォーマットのチェック
                    if (sections.Length < 1 || !(sections[0] == "fs"))
                    {
                        return false;
                    }

                    if (sections.Length < 2 || !(sections[1].Length == 1))
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
                    sb.Append(string.Join('\\', sections.Skip(2)));

                    path = sb.ToString();
                    consumed = sections.Length;

                    return true;
                }
            }
        }
    }
}
