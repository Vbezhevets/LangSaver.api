namespace LangSaver.Application.DTO;

public record WordResponse(
    Guid Id,
    string Term,
    string Language,
    string? Category,
    List<TranslationResponse> Translations
);

public record TranslationResponse(
    Guid Id,
    string Term,
    string Language,
    string? Category
);