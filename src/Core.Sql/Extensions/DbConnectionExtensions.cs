using System.Data.Common;

namespace Core.Sql;

public static class DbConnectionExtensions
{
    public static int ExecuteNonQuery(this DbConnection connection, string query)
    {
        return ExecuteNonQuery(connection, query, Array.Empty<(string, object?)>());
    }

    public static int ExecuteNonQuery(this DbConnection connection, string query, IEnumerable<(string, object?)> parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteNonQuery();
    }

    public static ValueTask<int> ExecuteNonQueryAsync(this DbConnection connection, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteNonQueryAsync(connection, query, Array.Empty<(string, object?)>(), cancellationToken);
    }

    public static async ValueTask<int> ExecuteNonQueryAsync(this DbConnection connection, string query, IEnumerable<(string, object?)> parameters, CancellationToken cancellationToken = default)
    {
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static object? ExecuteScalar(this DbConnection connection, string query)
    {
        return ExecuteScalar(connection, query, Array.Empty<(string, object?)>());
    }

    public static object? ExecuteScalar(this DbConnection connection, string query, IEnumerable<(string, object?)> parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteScalar();
    }

    public static ValueTask<object?> ExecuteScalarAsync(this DbConnection connection, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteScalarAsync(connection, query, Array.Empty<(string, object?)>(), cancellationToken);
    }

    public static async ValueTask<object?> ExecuteScalarAsync(this DbConnection connection, string query, IEnumerable<(string, object?)> parameters, CancellationToken cancellationToken = default)
    {
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteScalarAsync(cancellationToken);
    }
}
