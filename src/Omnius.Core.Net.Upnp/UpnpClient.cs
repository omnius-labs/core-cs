using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Omnius.Core.Net.Upnp
{
    public class UpnpClient : DisposableBase, IUpnpClient
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private string? _contents;
        private Uri? _location;

        private static readonly Regex _deviceTypeRegex = new Regex(@"<(\s*)deviceType((\s*)|(\s+)(.*?))>(\s*)urn:schemas-upnp-org:device:InternetGatewayDevice:1(\s*)</(\s*)deviceType(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex _controlUrlRegex = new Regex(@"<(\s*)controlURL((\s*)|(\s+)(.*?))>(\s*)(?<url>.*?)(\s*)</(\s*)controlURL(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        internal sealed class UpnpClientFactory : IUpnpClientFactory
        {
            public IUpnpClient Create()
            {
                return new UpnpClient();
            }
        }

        public static IUpnpClientFactory Factory { get; } = new UpnpClientFactory();

        internal UpnpClient()
        {
        }

        public bool IsConnected => _contents != null;

        public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());

                foreach (var machineIp in hostEntry.AddressList)
                {
                    if (machineIp.AddressFamily != AddressFamily.InterNetwork) continue;

                    (_contents, _location) = await GetContentsAndLocationFromDeviceAsync(IPAddress.Parse("239.255.255.250"), machineIp, cancellationToken);
                }
            }
            catch (UpnpClientException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UpnpClientException("Failed to connect.", e);
            }
        }

        private static async ValueTask<(string contents, Uri location)> GetContentsAndLocationFromDeviceAsync(IPAddress targetIp, IPAddress localIp, CancellationToken cancellationToken = default)
        {
            var querys = new List<string>
            {
                "M-SEARCH * HTTP/1.1\r\n" +
                "Host: 239.255.255.250:1900\r\n" +
                "Man: \"ssdp:discover\"\r\n" +
                "ST: urn:schemas-upnp-org:service:WANIPConnection:1\r\n" +
                "MX: 3\r\n" +
                "\r\n",

                "M-SEARCH * HTTP/1.1\r\n" +
                "Host: 239.255.255.250:1900\r\n" +
                "Man: \"ssdp:discover\"\r\n" +
                "ST: urn:schemas-upnp-org:service:WANPPPConnection:1\r\n" +
                "MX: 3\r\n" +
                "\r\n",
            };

            var random = new Random();

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                for (int i = 0; ; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        socket.Bind(new IPEndPoint(IPAddress.Any, random.Next(30000, 50000)));
                        break;
                    }
                    catch (SocketException e)
                    {
                        _logger.Info(e, "Failed to bind of socket.");
                        if (i > 10) throw new UpnpClientException("Failed to bind of socket.", e);
                    }
                }

                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localIp.GetAddressBytes());
                socket.ReceiveTimeout = 5000;
                socket.SendTimeout = 5000;

                for (int i = 0; i < querys.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var queryBytes = Encoding.ASCII.GetBytes(querys[i]);
                        var endPoint = new IPEndPoint(targetIp, 1900);

                        await socket.SendToAsync(new ArraySegment<byte>(queryBytes), SocketFlags.None, endPoint);
                    }
                    catch (SocketException e)
                    {
                        _logger.Info(e, "Failed to send of socket.");
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string queryResponse;

                    try
                    {
                        var data = new byte[1024 * 64];
                        int dataLength = socket.Receive(data.AsSpan(), SocketFlags.None);

                        queryResponse = Encoding.ASCII.GetString(data, 0, dataLength);
                        if (string.IsNullOrWhiteSpace(queryResponse)) continue;
                    }
                    catch (SocketException e)
                    {
                        _logger.Info(e, "Failed to receive of socket.");
                        continue;
                    }

                    try
                    {
                        string location = Regex.Match(queryResponse, "^location.*?:(.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Groups[1].Value.Trim();
                        if (string.IsNullOrWhiteSpace(location)) continue;

                        _logger.Debug("UPnP Router: " + targetIp.ToString());
                        _logger.Debug("UPnP Location: " + location);

                        string contexts;

                        using (var client = new HttpClient())
                        {
                            contexts = await client.GetStringAsync(location);
                        }

                        if (string.IsNullOrWhiteSpace(contexts)) continue;

                        if (!_deviceTypeRegex.IsMatch(contexts)) continue;

                        return (contexts, new Uri(location));
                    }
                    catch (HttpRequestException e)
                    {
                        _logger.Info(e, "Failed to http request.");
                    }
                }

                throw new UpnpClientException("Failed to find UPnP router.");
            }
        }

        private static async ValueTask<string?> GetExternalIpAddressFromContentsAsync(string contents, string serviceType, string gatewayIp, int gatewayPort, CancellationToken cancellationToken = default)
        {
            if (contents == null || !contents.Contains(serviceType)) return null;

            try
            {
                contents = contents.Substring(contents.IndexOf(serviceType));

                string controlUrl = _controlUrlRegex.Match(contents).Groups["url"].Value.Trim();
                string soapBody =
                    "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                    " <s:Body>" +
                    "  <u:GetExternalIPAddress xmlns:u=\"" + serviceType + "\">" + "</u:GetExternalIPAddress>" +
                    " </s:Body>" +
                    "</s:Envelope>";
                var uri = new Uri(new Uri("http://" + gatewayIp + ":" + gatewayPort.ToString()), controlUrl);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"" + serviceType + "#GetExternalIPAddress\"");

                    using (var response = await client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml"), cancellationToken))
                    using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        var regex = new Regex(@"<(\s*)NewExternalIPAddress((\s*)|(\s+)(.*?))>(\s*)(?<address>.*?)(\s*)<\/(\s*)NewExternalIPAddress(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        return regex.Match(reader.ReadToEnd()).Groups["address"].Value.Trim();
                    }
                }
            }
            catch (Exception e)
            {
                throw new UpnpClientException("Failed to get external ip address.", e);
            }
        }

        private static async ValueTask<bool> OpenPortFromContentsAsync(string contents, string serviceType, string gatewayIp, int gatewayPort, UpnpProtocolType protocol, string machineIp, int externalPort, int internalPort, string description, CancellationToken cancellationToken = default)
        {
            if (contents == null || !contents.Contains(serviceType)) return false;

            try
            {
                contents = contents.Substring(contents.IndexOf(serviceType));

                string controlUrl = _controlUrlRegex.Match(contents).Groups["url"].Value.Trim();
                string protocolString = "";

                if (protocol == UpnpProtocolType.Tcp)
                {
                    protocolString = "TCP";
                }
                else if (protocol == UpnpProtocolType.Udp)
                {
                    protocolString = "UDP";
                }

                string soapBody =
                    "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                    " <s:Body>" +
                    "  <u:AddPortMapping xmlns:u=\"" + serviceType + "\">" +
                    "   <NewRemoteHost></NewRemoteHost>" +
                    "   <NewExternalPort>" + externalPort + "</NewExternalPort>" +
                    "   <NewProtocol>" + protocolString + "</NewProtocol>" +
                    "   <NewInternalPort>" + internalPort + "</NewInternalPort>" +
                    "   <NewInternalClient>" + machineIp + "</NewInternalClient>" +
                    "   <NewEnabled>1</NewEnabled>" +
                    "   <NewPortMappingDescription>" + description + "</NewPortMappingDescription>" +
                    "   <NewLeaseDuration>0</NewLeaseDuration>" +
                    "  </u:AddPortMapping>" +
                    " </s:Body>" +
                    "</s:Envelope>";
                var uri = new Uri(new Uri("http://" + gatewayIp + ":" + gatewayPort.ToString()), controlUrl);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"" + serviceType + "#AddPortMapping\"");

                    using (var response = await client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml"), cancellationToken))
                    {
                        if (response.StatusCode == HttpStatusCode.OK) return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                throw new UpnpClientException("Failed to get external ip address.", e);
            }
        }

        private static async ValueTask<bool> ClosePortFromContentsAsync(string services, string serviceType, string gatewayIp, int gatewayPort, UpnpProtocolType protocol, int externalPort, CancellationToken cancellationToken = default)
        {
            if (services == null || !services.Contains(serviceType)) return false;

            try
            {
                services = services.Substring(services.IndexOf(serviceType));

                string controlUrl = _controlUrlRegex.Match(services).Groups["url"].Value.Trim();
                string protocolString = "";

                if (protocol == UpnpProtocolType.Tcp)
                {
                    protocolString = "TCP";
                }
                else if (protocol == UpnpProtocolType.Udp)
                {
                    protocolString = "UDP";
                }

                string soapBody =
                    "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                    " <s:Body>" +
                    "  <u:DeletePortMapping xmlns:u=\"" + serviceType + "\">" +
                    "   <NewRemoteHost></NewRemoteHost>" +
                    "   <NewExternalPort>" + externalPort + "</NewExternalPort>" +
                    "   <NewProtocol>" + protocolString + "</NewProtocol>" +
                    "  </u:DeletePortMapping>" +
                    " </s:Body>" +
                    "</s:Envelope>";
                var uri = new Uri(new Uri("http://" + gatewayIp + ":" + gatewayPort.ToString()), controlUrl);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"" + serviceType + "#DeletePortMapping\"");

                    using (var response = await client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml"), cancellationToken))
                    {
                        if (response.StatusCode == HttpStatusCode.OK) return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                throw new UpnpClientException("Failed to get external ip address.", e);
            }
        }

        private static async ValueTask<dynamic?> GetPortEntryFromContentsAsync(string services, string serviceType, string gatewayIp, int gatewayPort, int index, CancellationToken cancellationToken = default)
        {
            if (services == null || !services.Contains(serviceType)) return null;

            try
            {
                services = services.Substring(services.IndexOf(serviceType));

                string controlUrl = _controlUrlRegex.Match(services).Groups["url"].Value.Trim();
                string soapBody =
                    "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                    " <s:Body>" +
                    "  <u:GetGenericPortMappingEntry xmlns:u=\"" + serviceType + "\">" +
                    "   <NewPortMappingIndex>" + index + "</NewPortMappingIndex>" +
                    "  </u:GetGenericPortMappingEntry>" +
                    " </s:Body>" +
                    "</s:Envelope>";
                var uri = new Uri(new Uri("http://" + gatewayIp + ":" + gatewayPort.ToString()), controlUrl);

                var expandoObject = new ExpandoObject();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"" + serviceType + "#GetGenericPortMappingEntry\"");

                    using (var response = await client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml"), cancellationToken))
                    using (var xml = XmlReader.Create(response.Content.ReadAsStreamAsync().Result))
                    {
                        while (xml.Read())
                        {
                            if (xml.NodeType == XmlNodeType.Element)
                            {
                                if (xml.LocalName == "GetGenericPortMappingEntryResponse")
                                {
                                    using (var xmlSubtree = xml.ReadSubtree())
                                    {
                                        while (xmlSubtree.Read())
                                        {
                                            if (xmlSubtree.NodeType == XmlNodeType.Element)
                                            {
                                                expandoObject.TryAdd(xmlSubtree.LocalName, xmlSubtree.ReadContentAsString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return expandoObject;
            }
            catch (Exception e)
            {
                throw new UpnpClientException("Failed to get external ip address.", e);
            }
        }

        public async ValueTask<string> GetExternalIpAddressAsync(CancellationToken cancellationToken = default)
        {
            if (_contents == null || _location == null) throw new UpnpClientException(nameof(UpnpClient) + " is not connected");

            string? result;

            result = await GetExternalIpAddressFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, cancellationToken);
            if (result != null) return result;

            result = await GetExternalIpAddressFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, cancellationToken);
            if (result != null) return result;

            throw new UpnpClientException("Failed to get external ip address.");
        }

        public async ValueTask<bool> OpenPortAsync(UpnpProtocolType protocol, int externalPort, int internalPort, string description, CancellationToken cancellationToken = default)
        {
            if (_contents == null || _location == null) throw new UpnpClientException(nameof(UpnpClient) + " is not connected");

            foreach (var ipAddress in await Dns.GetHostAddressesAsync(Dns.GetHostName()))
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetwork) continue;

                if (await this.OpenPortAsync(protocol, ipAddress.ToString(), externalPort, internalPort, description, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        public async ValueTask<bool> OpenPortAsync(UpnpProtocolType protocol, string machineIp, int externalPort, int internalPort, string description, CancellationToken cancellationToken = default)
        {
            if (_contents == null || _location == null) throw new UpnpClientException(nameof(UpnpClient) + " is not connected");

            if (await OpenPortFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, protocol, machineIp, externalPort, internalPort, description, cancellationToken))
            {
                return true;
            }

            if (await OpenPortFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, protocol, machineIp, externalPort, internalPort, description, cancellationToken))
            {
                return true;
            }

            return false;
        }

        public async ValueTask<bool> ClosePortAsync(UpnpProtocolType protocol, int externalPort, CancellationToken cancellationToken = default)
        {
            if (_contents == null || _location == null) throw new UpnpClientException(nameof(UpnpClient) + " is not connected");

            if (await ClosePortFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, protocol, externalPort, cancellationToken))
            {
                return true;
            }

            if (await ClosePortFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, protocol, externalPort, cancellationToken))
            {
                return true;
            }

            return false;
        }

        public async ValueTask<dynamic> GetPortEntryAsync(int index, CancellationToken cancellationToken = default)
        {
            if (_contents == null || _location == null) throw new UpnpClientException(nameof(UpnpClient) + " is not connected");

            dynamic? result;

            result = await GetPortEntryFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, index, cancellationToken);
            if (!(result is null)) return result;

            result = await GetPortEntryFromContentsAsync(_contents, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, index, cancellationToken);
            if (!(result is null)) return result;

            throw new UpnpClientException("Failed to get port entry.");
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }

    public class UpnpClientException : Exception
    {
        public UpnpClientException()
            : base()
        {
        }

        public UpnpClientException(string message)
            : base(message)
        {
        }

        public UpnpClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
