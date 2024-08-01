using System.Text;
using System.Text.Json;

namespace Omnius.Core.Serialization;

public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(name[0]));

        for (int i = 1; i < name.Length; ++i)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
