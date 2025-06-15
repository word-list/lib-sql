using System.Runtime.CompilerServices;
using Npgsql;
using WordList.Data.Sql.Models;

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
            SslMode = SslMode.VerifyFull,
            SearchPath = "public"
        };

        _dataSource = NpgsqlDataSource.Create(builder.ToString());
    }

    public async IAsyncEnumerable<string> GetExistingWordsAsync(params string[] words)
    {
        await using var conn = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        await using var cmd = new NpgsqlCommand("SELECT text FROM words WHERE text = ANY(@words)", conn);
        cmd.Parameters.AddWithValue("words", words);

        await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            yield return reader.GetString(0);
        }
    }

    public async Task<UpsertWordsResult> UpsertWordsAsync(params Word[] words)
    {
        await using var conn = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        await using var transaction = await conn.BeginTransactionAsync().ConfigureAwait(false);

        // Update words
        await using var cmd = new NpgsqlCommand(@"
            UPSERT INTO words 
                (text, commonness, offensiveness, sentiment, 
                 formality, culturalsensitivity, figurativeness, 
                 complexity, political)
            SELECT * FROM UNNEST
                (@textArray, @commonnessArray, @offensivenessArray, @sentimentArray,
                 @formalityArray, @culturalsensitivityArray, @figurativenessArray,
                 @complexityArray, @politicalArray)", conn, transaction);

        void addParam<T>(string name, Func<Word, T> expr)
            => cmd.Parameters.AddWithValue(name, words.Select(expr).ToArray());

        addParam("textArray", w => w.Text);
        addParam("commonnessArray", w => w.Commonness);
        addParam("offensivenessArray", w => w.Offensiveness);
        addParam("sentimentArray", w => w.Sentiment);
        addParam("formalityArray", w => w.Formality);
        addParam("culturalsensitivityArray", w => w.CulturalSensitivity);
        addParam("figurativenessArray", w => w.Figurativeness);
        addParam("complexityArray", w => w.Complexity);
        addParam("politicalArray", w => w.Political);

        var modifiedWordsCount = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

        // Update word types
        var wordTypes = words.SelectMany(w => w.WordTypes).Distinct().ToArray();
        var wordWordTypes =
            from w in words
            from wt in w.WordTypes
            select new { Word = w.Text, Type = wt };

        await using var typeCmd = new NpgsqlCommand(@"
            UPSERT INTO word_types (name) 
            SELECT * FROM UNNEST(@typeArray)", conn, transaction);

        typeCmd.Parameters.AddWithValue("typeArray", wordTypes);

        var modifiedWordTypesCount = await typeCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

        // Update word type memberships
        await using var wordWordTypeCmd = new NpgsqlCommand(@"
            UPSERT INTO word_word_types (word_text, word_type_name)
            SELECT * FROM UNNEST(@wordTextArray, @wordTypeNameArray)", conn, transaction);

        wordWordTypeCmd.Parameters.AddWithValue("wordTextArray", wordWordTypes.Select(wt => wt.Word).ToArray());
        wordWordTypeCmd.Parameters.AddWithValue("wordTypeNameArray", wordWordTypes.Select(wt => wt.Type).ToArray());

        var modifiedWordWordTypesCount = await wordWordTypeCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

        await transaction.CommitAsync().ConfigureAwait(false);

        return new UpsertWordsResult
        {
            ModifiedWordsCount = modifiedWordsCount,
            ModifiedWordTypesCount = modifiedWordTypesCount,
            ModifiedWordWordTypesCount = modifiedWordWordTypesCount
        };
    }
}