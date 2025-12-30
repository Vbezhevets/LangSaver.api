
namespace LangSaver.Application.DTO;
public class WordCreateRequest : IWordLookupRequest
{
    public string Term {get; set; }
    public string FromLanguage {get; set; }
    public string ToLanguage   {get; set; }
    public string? Category {get; set; }

    public string Language => FromLanguage;
}

public class WordQueryRequest : IWordLookupRequest
{
    
    public string Term {get; set; }
    public string FromLanguage {get; set; }
    public string ToLanguage   {get; set; }
    public string? Category {get; set; }

    public string Language => FromLanguage;
}

public class WordPatchRequest
{
    public string? Category {get; set; }

}