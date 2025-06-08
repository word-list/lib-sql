namespace WordList.Data.Sql.Models;

public class WordType
{
    public required string Text { get; set; }

    public List<Word> Words { get; set; } = [];
}