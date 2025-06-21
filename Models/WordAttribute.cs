using Amazon.DynamoDBv2.DataModel;

public class WordAttribute
{
    [DynamoDBHashKey]
    public required string Name { get; set; }

    [DynamoDBProperty]
    public required string Display { get; set; }

    [DynamoDBProperty]
    public required string Description { get; set; }

    [DynamoDBProperty]
    public required string Prompt { get; set; }
}