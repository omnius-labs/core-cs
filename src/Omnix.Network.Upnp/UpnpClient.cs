using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Omnix.Base;

namespace Omnix.Net.Upnp
{
    public enum UpnpProtocolType
    {
        Tcp,
        Udp,
    }

    public class UpnpClient : DisposableBase
    {
        private string _services;
        private Uri _location;
        private readonly static Regex _deviceTypeRegex = new Regex(@"<(\s*)deviceType((\s*)|(\s+)(.*?))>(\s*)urn:schemas-upnp-org:device:InternetGatewayDevice:1(\s*)</(\s*)deviceType(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly static Regex _controlUrlRegex = new Regex(@"<(\s*)controlURL((\s*)|(\s+)(.*?))>(\s*)(?<url>.*?)(\s*)</(\s*)controlURL(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private readonly object _lockObject = new object();
        private volatile bool _disposed;

        public bool Connect(TimeSpan timeout)
        {
            lock (_lockObject)
            {
                try
                {
                    foreach (var machineIp in Dns.GetHostEntryAsync(Dns.GetHostName()).Result.AddressList)
                    {
                        if (machineIp.AddressFamily != AddressFamily.InterNetwork) continue;

                        _services = GetServicesFromDevice(out _location, IPAddress.Parse("239.255.255.250"), machineIp, timeout);
                        if (_services != null) return true;
                    }
                }
                catch (Exception)
                {

                }

                return false;
            }
        }

        public bool IsConnected
        {
            get
            {
                return (_services != null);
            }
        }

        private static TimeSpan TimeoutCheck(TimeSpan elapsedTime, TimeSpan timeout)
        {
            var value = timeout - elapsedTime;

            if (value > TimeSpan.Zero)
            {
                return value;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        private static string GetServicesFromDevice(out Uri location, IPAddress targetIp, IPAddress localIp, TimeSpan timeout)
        {
            location = null;

            var sw = Stopwatch.StartNew();

            var querys = new List<string>();

            //querys.Add("M-SEARCH * HTTP/1.1\r\n" +
            //        "Host: 239.255.255.250:1900\r\n" +
            //        "Man: \"ssdp:discover\"\r\n" +
            //        "ST: upnp:rootdevice\r\n" +
            //        "MX: 3\r\n" +
            //        "\r\n");

            querys.Add("M-SEARCH * HTTP/1.1\r\n" +
                    "Host: 239.255.255.250:1900\r\n" +
                    "Man: \"ssdp:discover\"\r\n" +
                    "ST: urn:schemas-upnp-org:service:WANIPConnection:1\r\n" +
                    "MX: 3\r\n" +
                    "\r\n");

            querys.Add("M-SEARCH * HTTP/1.1\r\n" +
                    "Host: 239.255.255.250:1900\r\n" +
                    "Man: \"ssdp:discover\"\r\n" +
                    "ST: urn:schemas-upnp-org:service:WANPPPConnection:1\r\n" +
                    "MX: 3\r\n" +
                    "\r\n");

            var random = new Random();
            var queryResponses = new List<string>();

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                for (; ; )
                {
                    TimeoutCheck(sw.Elapsed, timeout);

                    try
                    {
                        socket.Bind(new IPEndPoint(IPAddress.Any, random.Next(30000, 50000)));
                        break;
                    }
                    catch (Exception)
                    {

                    }
                }

                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localIp.GetAddressBytes());

                socket.ReceiveTimeout = (int)timeout.TotalMilliseconds;
                socket.SendTimeout = (int)timeout.TotalMilliseconds;

                for (int i = 0; i < querys.Count; i++)
                {
                    var q = Encoding.ASCII.GetBytes(querys[i]);

                    var endPoint = new IPEndPoint(targetIp, 1900);
                    socket.SendTo(q, q.Length, SocketFlags.None, endPoint);
                }

                try
                {
                    for (int i = 0; i < 1024; i++)
                    {
                        var data = new byte[1024 * 64];
                        int dataLength = socket.Receive(data);

                        string temp = Encoding.ASCII.GetString(data, 0, dataLength);
                        if (!string.IsNullOrWhiteSpace(temp)) queryResponses.Add(temp);
                    }
                }
                catch (Exception)
                {

                }
            }

            foreach (string queryResponse in queryResponses)
            {
                try
                {
                    string regexLocation = Regex.Match(queryResponse, "^location.*?:(.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(regexLocation)) continue;

                    var tempLocation = new Uri(regexLocation);

                    Debug.WriteLine("UPnP Router: " + targetIp.ToString());
                    Debug.WriteLine("UPnP Location: " + tempLocation.ToString());

                    string downloadString = null;

                    using (var client = new HttpClient())
                    {
                        downloadString = client.GetStringAsync(tempLocation).Result;
                    }

                    if (string.IsNullOrWhiteSpace(downloadString)) continue;
                    if (!_deviceTypeRegex.IsMatch(downloadString)) continue;

                    location = tempLocation;
                    return downloadString;
                }
                catch (Exception)
                {

                }
            }

            return null;
        }

        private static string GetExternalIpAddressFromService(string services, string serviceType, string gatewayIp, int gatewayPort, TimeSpan timeout)
        {
            if (services == null || !services.Contains(serviceType)) return null;

            try
            {
                services = services.Substring(services.IndexOf(serviceType));

                string controlUrl = _controlUrlRegex.Match(services).Groups["url"].Value.Trim();
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

                    using (var response = client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml")).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                            {
                                return Regex.Match(reader.ReadToEnd(), @"<(\s*)NewExternalIPAddress((\s*)|(\s+)(.*?))>(\s*)(?<address>.*?)(\s*)<\/(\s*)NewExternalIPAddress(\s*)>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups["address"].Value.Trim();
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {

            }

            return null;
        }

        private static bool OpenPortFromService(string services, string serviceType, string gatewayIp, int gatewayPort, UpnpProtocolType protocol, string machineIp, int externalPort, int internalPort, string description, TimeSpan timeout)
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

                    using (var response = client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml")).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {

            }

            return false;
        }

        private static bool ClosePortFromService(string services, string serviceType, string gatewayIp, int gatewayPort, UpnpProtocolType protocol, int externalPort, TimeSpan timeout)
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

                    using (var response = client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml")).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {

            }

            return false;
        }

        private static Information GetPortEntryFromService(string services, string serviceType, string gatewayIp, int gatewayPort, int index, TimeSpan timeout)
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

                var contexts = new List<InformationContext>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"" + serviceType + "#GetGenericPortMappingEntry\"");

                    using (var response = client.PostAsync(uri, new StringContent(soapBody, new UTF8Encoding(false), "text/xml")).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
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
                                                        contexts.Add(new InformationContext(xmlSubtree.LocalName, xmlSubtree.ReadContentAsString()));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return new Information(contexts);
            }
            catch (Exception)
            {

            }

            return null;
        }

        public string GetExternalIpAddress(TimeSpan timeout)
        {
            if (_services == null) throw new UpnpClientException();

            try
            {
                string value = GetExternalIpAddressFromService(_services, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, timeout);
                if (value != null) return value;
            }
            catch (Exception)
            {

            }
            try
            {
                string value = GetExternalIpAddressFromService(_services, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, timeout);
                if (value != null) return value;
            }
            catch (Exception)
            {

            }

            return null;
        }

        public bool OpenPort(UpnpProtocolType protocol, int externalPort, int internalPort, string description, TimeSpan timeout)
        {
            if (_services == null) throw new UpnpClientException();

            string hostname = Dns.GetHostName();

            foreach (var ipAddress in Dns.GetHostAddressesAsync(hostname).Result)
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetwork) continue;

                if (OpenPort(protocol, ipAddress.ToString(), externalPort, internalPort, description, timeout))
                {
                    return true;
                }
            }

            return false;
        }

        public bool OpenPort(UpnpProtocolType protocol, string machineIp, int externalPort, int internalPort, string description, TimeSpan timeout)
        {
            if (_services == null) throw new UpnpClientException();

            try
            {
                bool value = OpenPortFromService(_services, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, protocol, machineIp, externalPort, internalPort, description, timeout);
                if (value) return value;
            }
            catch (Exception)
            {

            }

            try
            {
                bool value = OpenPortFromService(_services, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, protocol, machineIp, externalPort, internalPort, description, timeout);
                if (value) return value;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public bool ClosePort(UpnpProtocolType protocol, int externalPort, TimeSpan timeout)
        {
            if (_services == null) throw new UpnpClientException();

            try
            {
                bool value = ClosePortFromService(_services, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, protocol, externalPort, timeout);
                if (value) return value;
            }
            catch (Exception)
            {

            }

            try
            {
                bool value = ClosePortFromService(_services, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, protocol, externalPort, timeout);
                if (value) return value;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public Information GetPortEntry(int index, TimeSpan timeout)
        {
            if (_services == null) throw new UpnpClientException();

            try
            {
                var value = GetPortEntryFromService(_services, "urn:schemas-upnp-org:service:WANIPConnection:1", _location.Host, _location.Port, index, timeout);
                if (value != null) return value;
            }
            catch (Exception)
            {

            }

            try
            {
                var value = GetPortEntryFromService(_services, "urn:schemas-upnp-org:service:WANPPPConnection:1", _location.Host, _location.Port, index, timeout);
                if (value != null) return value;
            }
            catch (Exception)
            {

            }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {

            }
        }
    }

    public class UpnpClientException : ManagerException
    {
        public UpnpClientException() : base() { }
        public UpnpClientException(string message) : base(message) { }
        public UpnpClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
