using System.Collections.Immutable;
using System.Text;
using Omnius.Core.Helpers;

namespace Omnius.Core.Net.I2p.Internal;

internal sealed class SamCommand : IEquatable<SamCommand>
{
    private readonly ImmutableList<string> _commands;
    private readonly ImmutableDictionary<string, string> _parameters;
    private readonly int _hashCode;

    public static SamCommand Empty { get; } = new SamCommand(new string[] { }, new KeyValuePair<string, string>[] { });

    public SamCommand(IEnumerable<string> commands, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        _commands = commands.ToImmutableList();
        _parameters = parameters.ToImmutableDictionary();

        var hashCode = new HashCode();
        foreach (var command in _commands)
        {
            hashCode.Add(command);
        }
        foreach (var parameter in _parameters)
        {
            hashCode.Add(parameter);
        }
        _hashCode = hashCode.ToHashCode();
    }

    public SamCommand(IEnumerable<string> commands, IEnumerable<ValueTuple<string, string>> parameters) : this(commands, parameters.Select(n => new KeyValuePair<string, string>(n.Item1, n.Item2)))
    {
    }

    public SamCommand(IEnumerable<string> commands) : this(commands, Enumerable.Empty<KeyValuePair<string, string>>())
    {
    }

    public void Deconstruct(out IReadOnlyList<string> commands, out IReadOnlyDictionary<string, string> parameters)
    {
        commands = this.Commands;
        parameters = this.Parameters;
    }

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object? other) => this.Equals(other as SamCommand);

    public bool Equals(SamCommand? other)
    {
        if (other is null) return false;
        if (object.ReferenceEquals(this, other)) return true;
        return CollectionHelper.Equals(this.Commands, other.Commands) && CollectionHelper.Equals(this.Parameters, other.Parameters);
    }

    public IReadOnlyList<string> Commands => _commands;

    public IReadOnlyDictionary<string, string> Parameters => _parameters;

    public static SamCommand Parse(string input)
    {
        var lines = Decode(input);

        if (lines.Length == 0)
        {
            return SamCommand.Empty;
        }
        else if (lines.Length == 1)
        {
            return new SamCommand(new[] { lines[0] });
        }
        else if (lines.Length == 2)
        {
            return new SamCommand(new[] { lines[0], lines[1] });
        }
        else if (lines.Length >= 3)
        {
            var commands = new[] { lines[0], lines[1] };
            var parameters = new Dictionary<string, string>();

            foreach (string pair in lines.Skip(2))
            {
                int equalsPosition = pair.IndexOf('=');

                string key;
                string value;

                if (equalsPosition == -1)
                {
                    key = pair;
                    value = string.Empty;
                }
                else
                {
                    key = pair.Substring(0, equalsPosition);
                    value = pair.Substring(equalsPosition + 1);
                }

                parameters.Add(key, value);
            }

            return new SamCommand(commands, parameters);
        }

        throw new FormatException();
    }

    private static string[] Decode(string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        var builder = new StringBuilder(input.Length);

        int begin = 0;
        int end;

        bool quoting = false;

        for (; ; )
        {
            end = input.IndexOf('\"', begin);
            if (end == -1) end = input.Length;

            string s = input.Substring(begin, end - begin);
            if (!quoting) s = s.Replace(' ', '\n');

            builder.Append(s);

            if (end == input.Length) break;
            begin = end + 1;

            quoting = !quoting;
        }

        return builder.ToString().Split('\n');
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (string command in _commands)
        {
            sb.AppendFormat("{0} ", command);
        }

        foreach (var (key, value) in _parameters)
        {
            if (value != null)
            {
                if (!value.Contains(" "))
                {
                    sb.AppendFormat("{0}={1} ", key, value);
                }
                else
                {
                    sb.AppendFormat("{0}=\"{1}\" ", key, value);
                }
            }
            else
            {
                sb.AppendFormat("{0} ", key);
            }
        }

        sb.Length = (sb.Length - 1);

        return sb.ToString();
    }
}
