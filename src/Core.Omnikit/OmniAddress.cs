using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Omnius.Core.Omnikit.Internal.Extensions;
using Sprache;

namespace Omnius.Core.Omnikit;

public record class OmniAddress
{
    public required string Value { get; init; }

    public static OmniAddress CreateI2p(string address)
    {
        return new OmniAddress { Value = $"i2p({address})" };
    }

    public static OmniAddress CreateTcp(IPAddress ipAddress, ushort port)
    {
        return ipAddress.AddressFamily switch
        {
            AddressFamily.InterNetwork => new OmniAddress { Value = $"tcp(ip4({ipAddress}),{port})" },
            AddressFamily.InterNetworkV6 => new OmniAddress { Value = $"tcp(ip6({ipAddress}),{port})" },
            _ => throw new NotSupportedException(),
        };
    }

    public static OmniAddress CreateTcp(string hostname, ushort port)
    {
        return new OmniAddress { Value = $"tcp(dns({hostname}),{port})" };
    }

    public static bool TryParseI2p(string value, [NotNullWhen(true)] out string? address)
    {
        address = null;

        var element = StringParser.Parse(value);
        if (element is null) return false;

        return ElementParser.TryParseI2p(element, out address);
    }

    public static bool TryParseTcp(bool nameResolving, string value, [NotNullWhen(true)] out IPAddress? ipAddress, out ushort port)
    {
        ipAddress = null;
        port = 0;

        var element = StringParser.Parse(value);
        if (element is null) return false;

        return ElementParser.TryParseTcp(element, out ipAddress, out port, nameResolving);
    }

    private static class ElementParser
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

            TryParseIp4(hostFunction, out ipAddress);

            if (ipAddress is null)
            {
                TryParseIp6(hostFunction, out ipAddress);
            }

            if (ipAddress is null && nameResolving)
            {
                TryParseDns(hostFunction, out ipAddress);
            }

            if (ipAddress is null) return false;

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

        public static bool TryParseDns(FunctionElement rootFunction, [NotNullWhen(true)] out IPAddress? ipAddress)
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
            catch (Exception)
            {
                return false;
            }
        }
    }

    private static class StringParser
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
            from element in _quotedStringLiteralParser.Or(_quotedStringLiteralParser).TokenWithSkipSpace()
            from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
            select new ConstantElement { Text = element };

        private static readonly Parser<FunctionElement> _functionElementParser =
            from name in _stringLiteralParser.TokenWithSkipSpace()
            from beginTag in Sprache.Parse.Char('(').TokenWithSkipSpace()
            from arguments in _functionElementParser.Or<object>(_constantElementParser).Many().TokenWithSkipSpace()
            from endTag in Sprache.Parse.Char(')').TokenWithSkipSpace()
            from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
            select new FunctionElement { Name = name, Arguments = arguments.ToArray() };

        public static FunctionElement? Parse(string text)
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

    private record FunctionElement
    {
        public required string Name { get; init; }
        public required IReadOnlyList<object> Arguments { get; init; }
    }

    private record ConstantElement
    {
        public required string Text { get; init; }
    }
}
