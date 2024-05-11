using System.Data.Common;

namespace Core.Sql;

public static class DbTransactionExtensions
{
    public static int ExecuteNonQuery(this DbTransaction transaction, string query)
    {
        return ExecuteNonQuery(transaction, query, Array.Empty<(string, object?)>());
    }

    public static int ExecuteNonQuery(this DbTransaction transaction, string query, IEnumerable<(string, object?)> parameters)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteNonQuery();
    }

    public static ValueTask<int> ExecuteNonQueryAsync(this DbTransaction transaction, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteNonQueryAsync(transaction, query, Array.Empty<(string, object?)>(), cancellationToken);
    }

    public static async ValueTask<int> ExecuteNonQueryAsync(this DbTransaction transaction, string query, IEnumerable<(string, object?)> parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static object? ExecuteScalar(this DbTransaction transaction, string query)
    {
        return ExecuteScalar(transaction, query, Array.Empty<(string, object?)>());
    }

    public static object? ExecuteScalar(this DbTransaction transaction, string query, IEnumerable<(string, object?)> parameters)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteScalar();
    }

    public static ValueTask<object?> ExecuteScalarAsync(this DbTransaction transaction, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteScalarAsync(transaction, query, Array.Empty<(string, object?)>(), cancellationToken);
    }

    public static async ValueTask<object?> ExecuteScalarAsync(this DbTransaction transaction, string query, IEnumerable<(string, object?)> parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteScalarAsync(cancellationToken);
    }
}
