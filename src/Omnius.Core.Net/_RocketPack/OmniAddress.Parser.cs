using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Omnius.Core.Collections;
using Omnius.Core.Net.Internal.Extensions;
using Sprache;

namespace Omnius.Core.Net
{
    public partial class OmniAddress
    {
        public static OmniAddress Parse(string text)
        {
            return new OmniAddress(text);
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

        public bool TryGetTcpEndpoint(out IPAddress ipAddress, out ushort port, bool nameResolving = false)
        {
            ipAddress = IPAddress.None;
            port = 0;

            var rootFunction = this.Deconstruct();

            if (rootFunction == null) return false;

            if (rootFunction.Name == "tcp")
            {
                if (!(rootFunction.Arguments.Count == 2
                    && rootFunction.Arguments[0] is FunctionElement hostFunction
                    && rootFunction.Arguments[1] is ConstantElement portConstant))
                {
                    return false;
                }

                if (hostFunction.Name == "ip4")
                {
                    if (!(hostFunction.Arguments.Count == 1
                        && hostFunction.Arguments[0] is ConstantElement ipAddressConstant))
                    {
                        return false;
                    }

                    if (!IPAddress.TryParse(ipAddressConstant.Text, out var temp)
                        || temp.AddressFamily != AddressFamily.InterNetwork)
                    {
                        return false;
                    }

                    ipAddress = temp;
                }
                else if (hostFunction.Name == "ip6")
                {
                    if (!(hostFunction.Arguments.Count == 1
                        && hostFunction.Arguments[0] is ConstantElement ipAddressConstant))
                    {
                        return false;
                    }

                    if (!IPAddress.TryParse(ipAddressConstant.Text, out var temp)
                        || temp.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        return false;
                    }

                    ipAddress = temp;
                }
                else if (nameResolving && hostFunction.Name == "dns")
                {
                    if (!(hostFunction.Arguments.Count == 1
                        && hostFunction.Arguments[0] is ConstantElement hostnameConstant))
                    {
                        return false;
                    }

                    try
                    {
                        var hostEntry = Dns.GetHostEntry(hostnameConstant.Text);

                        if (hostEntry.AddressList.Length == 0) return false;

                        ipAddress = hostEntry.AddressList[0];
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                if (!ushort.TryParse(portConstant.Text, out port)) return false;
            }

            return true;
        }

        private FunctionElement Deconstruct()
        {
            return Deconstructor.Deconstruct(this.Value);
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
                select string.Join(string.Empty, fragments).Replace("\\\"", "\"");

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

            public static FunctionElement Deconstruct(string text)
            {
                try
                {
                    return _functionElementParser.End().Parse(text);
                }
                catch (Exception e)
                {
                    _logger.Error(e);

                    throw new FormatException("Failed parse.", e);
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
}
