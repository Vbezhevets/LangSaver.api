
namespace LangSaver.Application.DTO;
public class WordCreateRequest : IWordLookupRequest
{
    public WordCreateRequest(string term, string fromLanguage, string toLanguage, string? category, string?translation)
    {
        Term = term;
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
        Translation = translation;
        Category = category;

    }

    public string Term { get; set; }
    public string FromLanguage { get; set; }
    public string ToLanguage { get; set; }
    public string? Translation { get; set; }
    public string? Category { get; set; }
    

    public string Language => FromLanguage;
}

public class WordQueryRequest : IWordLookupRequest
{
    public WordQueryRequest(string term, string fromLanguage, string toLanguage, string? category)
    {
        Term = term;
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
        Category = category;
    }

    public string Term { get; set; }
    public string FromLanguage { get; set; }
    public string ToLanguage { get; set; }
    public string? Category { get; set; }

    public string Language => FromLanguage;
}

public class WordPatchRequest
{
    public WordPatchRequest(string? category)
    {
        Category = category;
    }

    public string? Category { get; set; }

}