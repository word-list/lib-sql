namespace WordList.Data.Sql.Models;

public class Word
{
    public required string Text { get; set; }
    public string[] WordTypes { get; set; } = [];

    public IDictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();
}