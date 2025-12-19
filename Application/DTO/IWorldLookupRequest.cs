namespace LangSaver.Application.DTO;

public interface IWordLookupRequest
{
    string Term {get; }
    string Language {get; }
    string? Category {get; }
}