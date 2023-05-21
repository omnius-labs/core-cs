using System.Data.Common;

public static class DbConnectionExtensions
{
    public static int ExecuteNonQuery(this DbConnection connection, string query)
    {
        return ExecuteNonQuery(connection, query, Array.Empty<(string, object)>());
    }

    public static int ExecuteNonQuery(this DbConnection connection, string query, IEnumerable<(string, object)> parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteNonQuery();
    }

    public static ValueTask<int> ExecuteNonQueryAsync(this DbConnection connection, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteNonQueryAsync(connection, query, Array.Empty<(string, object)>(), cancellationToken);
    }

    public static async ValueTask<int> ExecuteNonQueryAsync(this DbConnection connection, string query, IEnumerable<(string, object)> parameters, CancellationToken cancellationToken = default)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
