namespace WordList.Data.Sql.Models;

public class Word
{
    public required string Text { get; set; }
    public int Commonness { get; set; }
    public int Offensiveness { get; set; }
    public int Sentiment { get; set; }

    public string[] WordTypes { get; set; } = [];


    // Added in v0.3.2
    public int Formality { get; set; }
    public int CulturalSensitivity { get; set; }
    public int Figurativeness { get; set; }
    public int Complexity { get; set; }
    public int Political { get; set; }
}