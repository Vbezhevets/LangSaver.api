using System.Net.Http.Json;
using LangSaver.Application.Interfaces;

namespace LangSaver.Application.Services;

public class LibreTranslateService : ITranslatorService
{
    private readonly HttpClient _httpClient;

    public LibreTranslateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> TranslateAsync(
        string text,
        string fromLanguage,
        string toLanguage)
    {
        var request = new LibreTranslateRequest
        {
            Q = text,
            Source = fromLanguage,
            Target = toLanguage,
            Format = "text"
        };

        var response = await _httpClient.PostAsJsonAsync("/translate", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();

        return result?.TranslatedText;
    }

    private class LibreTranslateRequest
    {
        public string Q { get; set; } = "";
        public string Source { get; set; } = "";
        public string Target { get; set; } = "";
        public string Format { get; set; } = "text";
    }

    private class LibreTranslateResponse
    {
        public string TranslatedText { get; set; } = "";
    }
}