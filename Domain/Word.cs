namespace LangSaver.Domain;
public class Word
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Term { get; set; }     // "run"
    public string Language { get; set; }     // "en"

    public List <Word> Translations { get; set; } = new();

    public string? Category { get; set; } // verbs, nouns, etc.

    public Guid UserId { get; set; }
    public User Owner { get; set; }
}
/*
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
пере
    public Guid UserId { get; set; }
    public User Owner {get; set; }

}
*/