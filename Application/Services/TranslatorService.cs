using LangSaver.Application.Interfaces;

namespace LangSaver.Application.Services;

public class TranslatorService : ITranslatorService
{
    public Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage)
    {
        return Task.FromResult <string?> (
            $"{text}_{toLanguage}");
    }

}