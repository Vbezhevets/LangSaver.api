namespace LangSaver.Application.Interfaces;

public interface ITranslatorService
{
    Task<string?> TranslateAsync (
        string text,
        string FromLanguage,
        string ToLanguage
    );
}