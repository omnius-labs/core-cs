using System.Data.Common;

public static class DbTransactionExtensions
{
    public static int ExecuteNonQuery(this DbTransaction transaction, string query)
    {
        return ExecuteNonQuery(transaction, query, Array.Empty<(string, object)>());
    }

    public static int ExecuteNonQuery(this DbTransaction transaction, string query, IEnumerable<(string, object)> parameters)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return command.ExecuteNonQuery();
    }

    public static ValueTask<int> ExecuteNonQueryAsync(this DbTransaction transaction, string query, CancellationToken cancellationToken = default)
    {
        return ExecuteNonQueryAsync(transaction, query, Array.Empty<(string, object)>(), cancellationToken);
    }

    public static async ValueTask<int> ExecuteNonQueryAsync(this DbTransaction transaction, string query, IEnumerable<(string, object)> parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        var command = transaction.Connection.CreateCommand();
        command.CommandText = query;
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
