using System.Data.Common;

namespace Core.Sql;

public static class CommandExtensions
{
    public static void AddParameters(this DbCommand command, IEnumerable<(string, object?)> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = key;
            p.Value = value;
            command.Parameters.Add(p);
        }
    }
}
