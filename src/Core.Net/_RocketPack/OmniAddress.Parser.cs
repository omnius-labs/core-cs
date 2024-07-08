using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Omnius.Core.Collections;
using Omnius.Core.Net.Internal.Extensions;
using Sprache;

namespace Omnius.Core.Net;

public partial class OmniAddress
{
    public static OmniAddress Parse(string? text)
    {
        if (text == null) return OmniAddress.Empty;
        return new OmniAddress(text);
    }

    public static OmniAddress CreateI2pEndpoint(string address)
    {
        return new OmniAddress($"i2p({address})");
    }

    public static OmniAddress CreateTcpEndpoint(string host, ushort port)
    {
        return new OmniAddress($"tcp(dns({host}),{port})");
    }

    public static OmniAddress CreateTcpEndpoint(IPAddress ipAddress, ushort port)
    {
        if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
        {
            return new OmniAddress($"tcp(ip4({ipAddress}),{port})");
        }
        else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return new OmniAddress($"tcp(ip4({ipAddress}),{port})");
        }
        else
        {
            throw new NotSupportedException($"AddressFamily is not supported");
        }
    }

    public bool TryParseI2pEndpoint([NotNullWhen(true)] out string? address)
    {
        address = null;

        var element = Deconstructor.Deconstruct(this.Value);
        if (element is null) return false;

        if (!Parser.TryParseI2p(element, out address))
        {
            return false;
        }

        return true;
    }

    public bool TryParseTcpEndpoint([NotNullWhen(true)] out IPAddress? ipAddress, out ushort port, bool nameResolving = false)
    {
        ipAddress = null;
        port = 0;

        var element = Deconstructor.Deconstruct(this.Value);
        if (element is null) return false;

        if (!Parser.TryParseTcp(element, out ipAddress, out port, nameResolving))
        {
            return false;
        }

        return true;
    }

    private static class Parser
    {
        public static bool TryParseI2p(FunctionElement rootFunction, [NotNullWhen(true)] out string? address)
        {
            address = null;

            if (rootFunction is not { Name: "i2p", Arguments: [ConstantElement hostConstant] })
            {
                return false;
            }

            address = hostConstant.Text;
            return true;
        }

        public static bool TryParseTcp(FunctionElement rootFunction, [NotNullWhen(true)] out IPAddress? ipAddress, out ushort port, bool nameResolving = false)
        {
            ipAddress = null;
            port = 0;

            if (rootFunction is not { Name: "tcp", Arguments: [FunctionElement hostFunction, ConstantElement portConstant] })
            {
                return false;
            }

            if (!TryParseIp4(hostFunction, out ipAddress)
                && !TryParseIp6(hostFunction, out ipAddress)
                && !TryParseDns(hostFunction, out ipAddress, nameResolving))
            {
                return false;
            }

            if (!ushort.TryParse(portConstant.Text, out port))
            {
                return false;
            }

            return true;
        }

        public static bool TryParseIp4(FunctionElement rootFunction, [NotNullWhen(true)] out IPAddress? ipAddress)
        {
            ipAddress = null;

            if (rootFunction is not { Name: "ip4", Arguments: [ConstantElement ip4Constant] })
            {
                return false;
            }

            if (!IPAddress.TryParse(ip4Constant.Text, out ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                return false;
            }

            return true;
        }

        public static bool TryParseIp6(FunctionElement rootFunction, [NotNullWhen(true)] out IPAddress? ipAddress)
        {
            ipAddress = null;

            if (rootFunction is not { Name: "ip6", Arguments: [ConstantElement ip6Constant] })
            {
                return false;
            }

            if (!IPAddress.TryParse(ip6Constant.Text, out ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return false;
            }

            return true;
        }

        public static bool TryParseDns(FunctionElement rootFunction, [NotNullWhen(true)] out IPAddress? ipAddress, bool nameResolving = false)
        {
            ipAddress = null;

            if (rootFunction is not { Name: "dns", Arguments: [ConstantElement hostnameConstant] })
            {
                return false;
            }

            try
            {
                var hostEntry = Dns.GetHostEntry(hostnameConstant.Text);
                if (hostEntry.AddressList.Length == 0) return false;

                ipAddress = hostEntry.AddressList[0];
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to DNS Resolve");
                return false;
            }
        }
    }

    private static class Deconstructor
    {
        private static readonly Parser<string> _stringLiteralParser =
            from name in Sprache.Parse.CharExcept(x => x == ',' || x == '(' || x == ')', "Name").AtLeastOnce().TokenWithSkipSpace().Text()
            select name;

        private static readonly Parser<string> _quotedStringLiteralParser =
            from openQuote in Sprache.Parse.Char('\"')
            from fragments in Sprache.Parse.Char('\\').Then(_ => Sprache.Parse.AnyChar.Select(c => $"\\{c}")).Or(Sprache.Parse.CharExcept("\\\"").Many().Text()).Many()
            from closeQuote in Sprache.Parse.Char('\"')
            select string.Join(string.Empty, fragments).Replace("\\\"", "\"", StringComparison.InvariantCulture);

        private static readonly Parser<ConstantElement> _constantElementParser =
            from element in _stringLiteralParser.Or(_quotedStringLiteralParser).TokenWithSkipSpace()
            from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
            select new ConstantElement(element);

        private static readonly Parser<FunctionElement> _functionElementParser =
            from name in _stringLiteralParser.TokenWithSkipSpace()
            from beginTag in Sprache.Parse.Char('(').TokenWithSkipSpace()
            from arguments in _functionElementParser.Or<object>(_constantElementParser).Many().TokenWithSkipSpace()
            from endTag in Sprache.Parse.Char(')').TokenWithSkipSpace()
            from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
            select new FunctionElement(name, arguments.ToArray());

        public static FunctionElement? Deconstruct(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            try
            {
                return _functionElementParser.End().Parse(text);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    private sealed class FunctionElement
    {
        public FunctionElement(string name, object[] arguments)
        {
            this.Name = name;
            this.Arguments = new ReadOnlyListSlim<object>(arguments);
        }

        public string Name { get; }

        public IReadOnlyList<object> Arguments { get; }
    }

    private sealed class ConstantElement
    {
        public ConstantElement(string text)
        {
            this.Text = text;
        }

        public string Text { get; }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
