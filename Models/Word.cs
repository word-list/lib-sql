namespace WordList.Data.Sql.Models;

public class Word
{
    public required string Text { get; set; }
    public int Commonness { get; set; }
    public int Offensiveness { get; set; }
    public int Sentiment { get; set; }

    public List<WordType> WordTypes { get; set; } = [];
}