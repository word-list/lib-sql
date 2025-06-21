namespace WordList.Data.Sql;

using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

public class WordAttributes
{
    private static readonly IDynamoDBContext s_dynamoDb = new DynamoDBContextBuilder().Build();

    private static Dictionary<string, WordAttribute> s_attributes = [];
    private static bool s_loaded = false;

    public static async Task LoadAsync()
    {
        var tableName = Environment.GetEnvironmentVariable("WORD_ATTRIBUTES_TABLE_NAME");
        var attributes = await s_dynamoDb.ScanAsync<WordAttribute>([], new ScanConfig { OverrideTableName = tableName }).GetRemainingAsync().ConfigureAwait(false);

        foreach (var attr in attributes)
        {
            s_attributes[attr.Name] = attr;
        }

        s_loaded = true;
    }

    public static async Task<List<WordAttribute>> GetAllAsync()
    {
        if (!s_loaded)
        {
            await LoadAsync().ConfigureAwait(false);
        }

        return s_attributes.Values.ToList();
    }

    public static async Task<WordAttribute?> GetAsync(string name)
    {
        if (!s_loaded)
        {
            await LoadAsync().ConfigureAwait(false);
        }

        s_attributes.TryGetValue(name, out var attribute);
        return attribute;
    }

}

[JsonSerializable(typeof(WordAttribute))]
public partial class WordAttributeSerializer : JsonSerializerContext
{
}