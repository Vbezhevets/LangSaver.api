using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LangSaver.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LangSaver.Application.Services;

public class LibreTranslateService : ITranslatorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public LibreTranslateService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["LibreTranslate:ApiKey"] ?? "";
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
            Format = "text",
            ApiKey = _apiKey
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"LibreTranslate URL: {_httpClient.BaseAddress}translate");
        Console.WriteLine($"LibreTranslate request: {json}");

        Console.WriteLine($"LibreTranslate BaseAddress: {_httpClient.BaseAddress}");
Console.WriteLine($"Text: {text}");
Console.WriteLine($"From: {fromLanguage}");
Console.WriteLine($"To: {toLanguage}");

        var response = await _httpClient.PostAsJsonAsync("/translate", request);
        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"LibreTranslate status: {(int)response.StatusCode}");
        Console.WriteLine($"LibreTranslate response: {responseBody}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"LibreTranslate error: {(int)response.StatusCode} {responseBody}");

        var result = JsonSerializer.Deserialize<LibreTranslateResponse>(responseBody);

        if (result == null || string.IsNullOrWhiteSpace(result.TranslatedText))
            throw new Exception($"LibreTranslate returned empty result. Body: {responseBody}");

        return result.TranslatedText;
    }

    private class LibreTranslateRequest
    {
        [JsonPropertyName("q")]
        public string Q { get; set; } = "";

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";

        [JsonPropertyName("target")]
        public string Target { get; set; } = "";

        [JsonPropertyName("format")]
        public string Format { get; set; } = "text";

        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; } = "";
    }

    private class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string TranslatedText { get; set; } = "";
    }
}

// const res = await fetch("http://127.0.0.1:5000/translate", {
// 	method: "POST",
// 	body: JSON.stringify({
// 		q: "how",
// 		source: "auto",
// 		target: "ru",
// 		format: "text",
// 		alternatives: 3,
// 		api_key: ""
// 	}),
// 	headers: { "Content-Type": "application/json" }
// });

// console.log(await res.json());