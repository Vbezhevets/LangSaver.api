namespace LangSaver.Domain;
public class WordPair
/* {
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Term1 { get; set; }     // "run"
    public string Lang1 { get; set; }     // "en"

    public string Term2 { get; set; }     // "laufen"
    public string Lang2 { get; set; }     // "de"

    public string? Category { get; set; } // verbs, nouns, etc.

    public Guid UserId { get; set; }
    public User Owner { get; set; }
} */

public class Word
{
    public Guid Id {get; set; } = Guid.NewGuid();

    public string Name {get; set; }
    public string Translation {get; set; }
    public string? Category {get; set; } = null;

    public string From {get; set; }
    public string  To {get; set; }

    public Guid UserId {get; set; }
    public User Owner {get; set; }

}*/
/* 
namespace LangSaver.Domain;
public class WordEntry
{
    public Guid Id {get; set; } = Guid.NewGuid();
    public string Term {get; set; }
    public string Language {get; set; }

    public string? Category {get; set; } = null;
    public Guid ConceptId {get; set; }

    public Concept Concept {get; set; }

    public Guid UserId { get; set; }
    public User Owner {get; set; }

}
*/