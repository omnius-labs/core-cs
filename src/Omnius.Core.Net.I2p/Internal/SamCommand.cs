using System.Collections.Immutable;
using System.Text;

namespace Omnius.Core.Net.I2p.Internal;

internal sealed class SamCommand
{
    private readonly ImmutableList<string> _commands;
    private readonly ImmutableDictionary<string, string?> _parameters;

    public static SamCommand Empty { get; } = new SamCommand(new string[] { }, new KeyValuePair<string, string?>[] { });

    public SamCommand(IEnumerable<string> commands, IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        _commands = commands.ToImmutableList();
        _parameters = parameters.ToImmutableDictionary();
    }

    public void Deconstruct(out IReadOnlyList<string> commands, out IReadOnlyDictionary<string, string?> parameters)
    {
        commands = this.Commands;
        parameters = this.Parameters;
    }

    public IReadOnlyList<string> Commands => _commands;

    public IReadOnlyDictionary<string, string?> Parameters => _parameters;

    public static SamCommand Parse(string input)
    {
        var commands = new List<string>();
        var parameters = new Dictionary<string, string?>();

        var lines = Decode(input);
        commands.Add(lines[0]);
        commands.Add(lines[1]);

        foreach (string pair in lines.Skip(2))
        {
            int equalsPosition = pair.IndexOf('=');

            string? key;
            string? value;

            if (equalsPosition == -1)
            {
                key = pair;
                value = null;
            }
            else
            {
                key = pair.Substring(0, equalsPosition);
                value = pair.Substring(equalsPosition + 1);
            }

            key = !(string.IsNullOrWhiteSpace(key)) ? key : null;
            value = !(string.IsNullOrWhiteSpace(value)) ? value : null;

            if (key is null) continue;

            parameters.Add(key, value);
        }

        return new SamCommand(commands, parameters);
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
