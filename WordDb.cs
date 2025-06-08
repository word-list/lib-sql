using Npgsql;

namespace WordList.Data.Sql;

public class WordDb
{
    private readonly NpgsqlDataSource _dataSource;

    public WordDb()
    {
        var envConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (string.IsNullOrEmpty(envConnectionString))
            throw new InvalidOperationException("DB_CONNECTION_STRING must be set");

        // Convert from a CockroachDB connection string
        var connectionStringUri = new Uri(envConnectionString);

        var userInfo = Uri.UnescapeDataString(connectionStringUri.UserInfo).Split(':');

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = connectionStringUri.Host,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = connectionStringUri.LocalPath.Trim('/'),
            Port = connectionStringUri.Port,
            Timeout = 120,
            SslMode = SslMode.VerifyFull
        };

        _dataSource = NpgsqlDataSource.Create(builder.ToString());
    }

    public async IAsyncEnumerable<string> GetExistingWordsAsync(params string[] words)
    {
        await using var conn = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        await using var cmd = new NpgsqlCommand("SELECT Text FROM Words WHERE Text = ANY(@words)", conn);
        cmd.Parameters.AddWithValue("words", words);

        await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            yield return reader.GetString(0);
        }
    }
}