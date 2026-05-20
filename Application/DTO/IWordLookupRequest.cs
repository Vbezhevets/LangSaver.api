namespace LangSaver.Application.DTO;

public interface IWordLookupRequest
{
    string Term {get; }
    string Language {get; }
    string? Category {get; }
}

/*
introduced IWordLookupRequest to avoid duplicating lookup logic. 
Several request types need the same minimal data: term, language and category. 
Thanks to this interface, methods like FindExistingWord and CreateWord 
can work with WordCreateRequest, WordQueryRequest and an internal lookup object 
for translated words.
*/