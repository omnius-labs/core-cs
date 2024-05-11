namespace Core.Sql;

public static class SqliteQueryHelper
{
    public static string EscapeText(string text)
    {
        return text.Replace("'", "''");
    }
}
