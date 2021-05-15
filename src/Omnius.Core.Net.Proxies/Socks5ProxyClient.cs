using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Proxies
{
    public class Socks5ProxyClient : ISocks5ProxyClient
    {
        private const int SOCKS5_DEFAULT_PORT = 1080;

        private const byte SOCKS5_VERSION_NUMBER = 5;
        private const byte SOCKS5_RESERVED = 0x00;
        private const byte SOCKS5_AUTH_NUMBER_OF_AUTH_METHODS_SUPPORTED = 2;
        private const byte SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED = 0x00;
        private const byte SOCKS5_AUTH_METHOD_GSSAPI = 0x01;
        private const byte SOCKS5_AUTH_METHOD_USERNAME_PASSWORD = 0x02;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN = 0x03;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_END = 0x7f;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_BEGIN = 0x80;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_END = 0xfe;
        private const byte SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS = 0xff;
        private const byte SOCKS5_CMD_CONNECT = 0x01;
        private const byte SOCKS5_CMD_BIND = 0x02;
        private const byte SOCKS5_CMD_UDP_ASSOCIATE = 0x03;
        private const byte SOCKS5_CMD_REPLY_SUCCEEDED = 0x00;
        private const byte SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE = 0x01;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
        private const byte SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE = 0x03;
        private const byte SOCKS5_CMD_REPLY_HOST_UNREACHABLE = 0x04;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_REFUSED = 0x05;
        private const byte SOCKS5_CMD_REPLY_TTL_EXPIRED = 0x06;
        private const byte SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED = 0x07;
        private const byte SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED = 0x08;
        private const byte SOCKS5_ADDRTYPE_IPV4 = 0x01;
        private const byte SOCKS5_ADDRTYPE_DOMAIN_NAME = 0x03;
        private const byte SOCKS5_ADDRTYPE_IPV6 = 0x04;

        private readonly string _destinationHost;
        private readonly int _destinationPort;
        private readonly string? _proxyUsername;
        private readonly string? _proxyPassword;

        private readonly object _lockObject = new();

        internal sealed class Socks5ProxyClientFactory : ISocks5ProxyClientFactory
        {
            public ISocks5ProxyClient Create(string destinationHost, int destinationPort)
            {
                return new Socks5ProxyClient(destinationHost, destinationPort);
            }

            public ISocks5ProxyClient Create(string proxyUsername, string proxyPassword, string destinationHost, int destinationPort)
            {
                return new Socks5ProxyClient(proxyUsername, proxyPassword, destinationHost, destinationPort);
            }
        }

        public static ISocks5ProxyClientFactory Factory { get; } = new Socks5ProxyClientFactory();

        internal Socks5ProxyClient(string destinationHost, int destinationPort)
        {
            if (string.IsNullOrEmpty(destinationHost)) throw new ArgumentNullException(nameof(destinationHost));
            else if (destinationPort <= 0 || destinationPort > 65535) throw new ArgumentOutOfRangeException(nameof(destinationPort), "port must be greater than zero and less than 65535");

            _destinationHost = destinationHost;
            _destinationPort = destinationPort;
        }

        public Socks5ProxyClient(string proxyUsername, string proxyPassword, string destinationHost, int destinationPort)
            : this(destinationHost, destinationPort)
        {
            _proxyUsername = proxyUsername;
            _proxyPassword = proxyPassword;
        }

        public async ValueTask ConnectAsync(Socket socket, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken).ConfigureAwait(false);

            try
            {
                // negotiate which authentication methods are supported / accepted by the server
                this.NegotiateServerAuthMethod(socket);

                // send a connect command to the proxy server for destination host and port
                this.SendCommand(socket, SOCKS5_CMD_CONNECT, _destinationHost, _destinationPort);
            }
            catch (Exception ex)
            {
                var remoteAddress = ((System.Net.IPEndPoint)socket.RemoteEndPoint!).Address?.ToString();
                var remotePort = ((System.Net.IPEndPoint)socket.RemoteEndPoint!).Port.ToString();
                throw new ProxyClientException($"Connection to proxy host {remoteAddress} on port {remotePort} failed.", ex);
            }
        }

        private void NegotiateServerAuthMethod(Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            {
                // SERVER AUTHENTICATION REQUEST
                // The client connects to the server, and sends a version
                // identifier/method selection message:
                //
                //      +----+----------+----------+
                //      |VER | NMETHODS | METHODS  |
                //      +----+----------+----------+
                //      | 1  |    1     | 1 to 255 |
                //      +----+----------+----------+

                var authRequest = new byte[4];
                authRequest[0] = SOCKS5_VERSION_NUMBER;
                authRequest[1] = SOCKS5_AUTH_NUMBER_OF_AUTH_METHODS_SUPPORTED;
                authRequest[2] = SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED;
                authRequest[3] = SOCKS5_AUTH_METHOD_USERNAME_PASSWORD;

                // send the request to the server specifying authentication types supported by the client.
                stream.Write(authRequest, 0, authRequest.Length);

                // SERVER AUTHENTICATION RESPONSE
                // The server selects from one of the methods given in METHODS, and
                // sends a METHOD selection message:
                //
                //     +----+--------+
                //     |VER | METHOD |
                //     +----+--------+
                //     | 1  |   1    |
                //     +----+--------+
                //
                // If the selected METHOD is X'FF', none of the methods listed by the
                // client are acceptable, and the client MUST close the connection.
                //
                // The values currently defined for METHOD are:
                //  * X'00' NO AUTHENTICATION REQUIRED
                //  * X'01' GSSAPI
                //  * X'02' USERNAME/PASSWORD
                //  * X'03' to X'7F' IANA ASSIGNED
                //  * X'80' to X'FE' RESERVED FOR PRIVATE METHODS
                //  * X'FF' NO ACCEPTABLE METHODS

                // receive the server response
                var response = new byte[2];
                stream.Read(response, 0, response.Length);

                // the first byte contains the socks version number (e.g. 5)
                // the second byte contains the auth method acceptable to the proxy server
                byte acceptedAuthMethod = response[1];

                // if the server does not accept any of our supported authenication methods then throw an error
                if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS) throw new ProxyClientException("The proxy destination does not accept the supported proxy client authentication methods.");

                if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD)
                {
                    if (_proxyUsername is null || _proxyPassword is null) throw new ProxyClientException("The proxy destination requires a username and password for authentication.");

                    // USERNAME / PASSWORD SERVER REQUEST
                    // Once the SOCKS V5 server has started, and the client has selected the
                    // Username/Password Authentication protocol, the Username/Password
                    // subnegotiation begins.  This begins with the client producing a
                    // Username/Password request:
                    //
                    //       +----+------+----------+------+----------+
                    //       |VER | ULEN |  UNAME   | PLEN |  PASSWD  |
                    //       +----+------+----------+------+----------+
                    //       | 1  |  1   | 1 to 255 |  1   | 1 to 255 |
                    //       +----+------+----------+------+----------+

                    var credentials = new byte[_proxyUsername.Length + _proxyPassword.Length + 3];
                    credentials[0] = SOCKS5_VERSION_NUMBER;
                    credentials[1] = (byte)_proxyUsername.Length;
                    Array.Copy(Encoding.ASCII.GetBytes(_proxyUsername), 0, credentials, 2, _proxyUsername.Length);
                    credentials[_proxyUsername.Length + 2] = (byte)_proxyPassword.Length;
                    Array.Copy(Encoding.ASCII.GetBytes(_proxyPassword), 0, credentials, _proxyUsername.Length + 3, _proxyPassword.Length);

                    // USERNAME / PASSWORD SERVER RESPONSE
                    // The server verifies the supplied UNAME and PASSWD, and sends the
                    // following response:
                    //
                    //   +----+--------+
                    //   |VER | STATUS |
                    //   +----+--------+
                    //   | 1  |   1    |
                    //   +----+--------+
                    //
                    // A STATUS field of X'00' indicates success. If the server returns a
                    // `failure' (STATUS value other than X'00') status, it MUST close the
                    // connection.
                    stream.Write(credentials, 0, credentials.Length);
                    var crResponse = new byte[2];
                    stream.Read(crResponse, 0, crResponse.Length);

                    if (crResponse[1] != 0)
                    {
                        socket.Dispose();
                        throw new ProxyClientException("Proxy authentification failure!");
                    }
                }
            }
        }

        private byte GetDestAddressType(string host)
        {
            if (!IPAddress.TryParse(host, out var ipAddress)) return SOCKS5_ADDRTYPE_DOMAIN_NAME;

            return ipAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => SOCKS5_ADDRTYPE_IPV4,
                AddressFamily.InterNetworkV6 => SOCKS5_ADDRTYPE_IPV6,
                _ => throw new ProxyClientException($"The host addess {host} of type '{Enum.GetName(typeof(AddressFamily), ipAddress.AddressFamily)}' is not a supported address type.  The supported types are InterNetwork and InterNetworkV6."),
            };
        }

        private byte[] GetDestAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case SOCKS5_ADDRTYPE_IPV4:
                case SOCKS5_ADDRTYPE_IPV6:
                    return IPAddress.Parse(host).GetAddressBytes();
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    // create a byte array to hold the host name bytes plus one byte to store the length
                    var bytes = new byte[host.Length + 1];

                    // if the address field contains a fully-qualified domain name.  The first
                    // octet of the address field contains the number of octets of name that
                    // follow, there is no terminating NUL octet.
                    bytes[0] = System.Convert.ToByte(host.Length);
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);
                    return bytes;
                default:
                    throw new ProxyClientException($"Not supported address type: {addressType}");
            }
        }

        private byte[] GetDestPortBytes(int value)
        {
            byte[] buffer = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)value);

            return buffer;
        }

        private void SendCommand(Socket socket, byte command, string destinationHost, int destinationPort)
        {
            using (var stream = new NetworkStream(socket))
            {
                byte addressType = this.GetDestAddressType(destinationHost);
                var destAddr = this.GetDestAddressBytes(addressType, destinationHost);
                var destPort = this.GetDestPortBytes(destinationPort);

                // The connection request is made up of 6 bytes plus the
                // length of the variable address byte array
                //
                //  +----+-----+-------+------+----------+----------+
                //  |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
                //  +----+-----+-------+------+----------+----------+
                //  | 1  |  1  | X'00' |  1   | Variable |    2     |
                //  +----+-----+-------+------+----------+----------+
                //
                // * VER protocol version: X'05'
                // * CMD
                //   * CONNECT X'01'
                //   * BIND X'02'
                //   * UDP ASSOCIATE X'03'
                // * RSV RESERVED
                // * ATYP address itemType of following address
                //   * IP V4 address: X'01'
                //   * DOMAINNAME: X'03'
                //   * IP V6 address: X'04'
                // * DST.ADDR desired destination address
                // * DST.PORT desired destination port in network octet order

                var request = new byte[4 + destAddr.Length + 2];
                request[0] = SOCKS5_VERSION_NUMBER;
                request[1] = command;
                request[2] = SOCKS5_RESERVED;
                request[3] = addressType;
                destAddr.CopyTo(request, 4);
                destPort.CopyTo(request, 4 + destAddr.Length);

                // send connect request.
                stream.Write(request, 0, request.Length);

                // PROXY SERVER RESPONSE
                //  +----+-----+-------+------+----------+----------+
                //  |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
                //  +----+-----+-------+------+----------+----------+
                //  | 1  |  1  | X'00' |  1   | Variable |    2     |
                //  +----+-----+-------+------+----------+----------+
                //
                // * VER protocol version: X'05'
                // * REP Reply field:
                //   * X'00' succeeded
                //   * X'01' general SOCKS server failure
                //   * X'02' connection not allowed by ruleset
                //   * X'03' Network unreachable
                //   * X'04' Host unreachable
                //   * X'05' Connection refused
                //   * X'06' TTL expired
                //   * X'07' Command not supported
                //   * X'08' Address itemType not supported
                //   * X'09' to X'FF' unassigned
                // * RSV RESERVED
                // * ATYP address itemType of following address

                var response = new byte[255];

                // read proxy server response
                stream.Read(response, 0, response.Length);

                byte replyCode = response[1];

                // evaluate the reply code for an error condition
                if (replyCode != SOCKS5_CMD_REPLY_SUCCEEDED)
                {
                    this.HandleProxyCommandError(response, destinationHost, destinationPort);
                }
            }
        }

        private void HandleProxyCommandError(byte[] response, string destinationHost, int destinationPort)
        {
            byte replyCode = response[1];
            byte addrType = response[3];
            string addr = "";
            short port = 0;

            switch (addrType)
            {
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    int addrLen = System.Convert.ToInt32(response[4]);
                    var addrBytes = new byte[addrLen];
                    for (int i = 0; i < addrLen; i++)
                    {
                        addrBytes[i] = response[i + 5];
                    }

                    addr = Encoding.ASCII.GetString(addrBytes);
                    var portBytesDomain = new byte[2];
                    portBytesDomain[0] = response[6 + addrLen];
                    portBytesDomain[1] = response[5 + addrLen];
                    port = BitConverter.ToInt16(portBytesDomain, 0);
                    break;

                case SOCKS5_ADDRTYPE_IPV4:
                    var ipv4Bytes = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        ipv4Bytes[i] = response[i + 4];
                    }

                    var ipv4 = new IPAddress(ipv4Bytes);
                    addr = ipv4.ToString();
                    var portBytesIpv4 = new byte[2];
                    portBytesIpv4[0] = response[9];
                    portBytesIpv4[1] = response[8];
                    port = BitConverter.ToInt16(portBytesIpv4, 0);
                    break;

                case SOCKS5_ADDRTYPE_IPV6:
                    var ipv6Bytes = new byte[16];
                    for (int i = 0; i < 16; i++)
                    {
                        ipv6Bytes[i] = response[i + 4];
                    }

                    var ipv6 = new IPAddress(ipv6Bytes);
                    addr = ipv6.ToString();
                    var portBytesIpv6 = new byte[2];
                    portBytesIpv6[0] = response[21];
                    portBytesIpv6[1] = response[20];
                    port = BitConverter.ToInt16(portBytesIpv6, 0);
                    break;
            }

            string proxyErrorText = replyCode switch
            {
                SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE => "a general socks destination failure occurred",
                SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET => "the connection is not allowed by proxy destination rule set",
                SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE => "the network was unreachable",
                SOCKS5_CMD_REPLY_HOST_UNREACHABLE => "the host was unreachable",
                SOCKS5_CMD_REPLY_CONNECTION_REFUSED => "the connection was refused by the remote network",
                SOCKS5_CMD_REPLY_TTL_EXPIRED => "the time to live (TTL) has expired",
                SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED => "the command issued by the proxy client is not supported by the proxy destination",
                SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED => "the address type specified is not supported",
                _ => $"that an unknown reply with the code value '{replyCode}' was received by the destination",
            };
            string exceptionMessage = $"The {proxyErrorText} concerning destination host {destinationHost} port number {destinationPort}.  The destination reported the host as {addr} port {port}";

            throw new ProxyClientException(exceptionMessage);
        }
    }
}
