using LangSaver.Application.DTO;
using LangSaver.Application.Exceptions;
using LangSaver.Application.Interfaces;
using LangSaver.Domain;
using Microsoft.EntityFrameworkCore;

namespace LangSaver.Application.Services;

public class WordService : IWordService
{
    private readonly LangSaverDbContext _db;
    private readonly ITranslatorService _translator;

    private record Lookup(string Term, string Language, string? Category) : IWordLookupRequest;

    public WordService(LangSaverDbContext db, ITranslatorService translator)
    {
        _db = db;
        _translator = translator;
    }

    public async Task<WordResponse> CreateAsync(Guid userId, WordCreateRequest request)
    {
        var existingWord = await FindExistingWord(userId, request);

        if (existingWord != null)
            throw new Exception($"Word already exists with id {existingWord.Id}");

        var translatedText = await _translator.TranslateAsync(
            request.Term,
            request.FromLanguage,
            request.ToLanguage
        );

        if (string.IsNullOrWhiteSpace(translatedText))
            throw new TranslationFailedException("Translation failed");

        var sourceWord = CreateWord(userId, request);

        var translatedWord = await FindOrCreateTranslatedWord(
            userId,
            new Lookup(
                translatedText,
                request.ToLanguage,
                request.Category
            )
        );

        await _db.Words.AddAsync(sourceWord);
        await _db.SaveChangesAsync();

        sourceWord.Translations.Add(translatedWord);
        translatedWord.Translations.Add(sourceWord);

        await _db.SaveChangesAsync();

        return ToResponse(sourceWord);
    }

    public async Task<WordResponse?> QueryAsync(Guid userId, WordQueryRequest request)
    {
        var sourceWord = await FindExistingWord(userId, request);

        if (sourceWord == null)
            return null;

        var translations = await _db.Words
            .AsNoTracking()
            .Where(candidate =>
                candidate.UserId == userId &&
                candidate.Language == request.ToLanguage &&
                candidate.Translations.Any(translation =>
                    translation.Id == sourceWord.Id))
            .ToListAsync();

        var result = new Word
        {
            Id = sourceWord.Id,
            UserId = sourceWord.UserId,
            Term = sourceWord.Term,
            Language = sourceWord.Language,
            Category = sourceWord.Category,
            Translations = translations
        };

        return ToResponse(result);
    }

    public async Task<WordResponse?> GetByIdAsync(Guid userId, Guid id)
    {
        var word = await _db.Words
            .AsNoTracking()
            .Include(w => w.Translations)
            .FirstOrDefaultAsync(w =>
                w.UserId == userId &&
                w.Id == id);

        return ToNullableResponse(word);
    }

    public async Task<WordResponse?> PatchAsync(Guid userId, Guid id, WordPatchRequest request)
    {
        var word = await _db.Words
            .Include(w => w.Translations)
            .FirstOrDefaultAsync(w =>
                w.UserId == userId &&
                w.Id == id);

        if (word == null)
            return null;

        if (request.Category != null)
            word.Category = request.Category;

        await _db.SaveChangesAsync();

        return ToResponse(word);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var existingWord = await _db.Words
            .FirstOrDefaultAsync(w =>
                w.UserId == userId &&
                w.Id == id);

        if (existingWord == null)
            return false;

        _db.Words.Remove(existingWord);
        await _db.SaveChangesAsync();

        return true;
    }

    private async Task<Word?> FindExistingWord(Guid userId, IWordLookupRequest request)
    {
        return await _db.Words.FirstOrDefaultAsync(w =>
            w.UserId == userId &&
            w.Language == request.Language &&
            EF.Functions.ILike(w.Term, request.Term) &&
            w.Category == request.Category
        );
    }

    private async Task<Word> FindOrCreateTranslatedWord(Guid userId, IWordLookupRequest request)
    {
        var existingWord = await FindExistingWord(userId, request);

        if (existingWord != null)
            return existingWord;

        var word = CreateWord(userId, request);

        await _db.Words.AddAsync(word);

        return word;
    }

    private static Word CreateWord(Guid userId, IWordLookupRequest request)
    {
        return new Word
        {
            UserId = userId,
            Term = request.Term,
            Language = request.Language,
            Category = request.Category
        };
    }

    private static WordResponse? ToNullableResponse(Word? word)
    {
        return word == null
            ? null
            : ToResponse(word);
    }

    private static WordResponse ToResponse(Word word)
    {
        return new WordResponse(
            word.Id,
            word.Term,
            word.Language,
            word.Category,
            word.Translations
                .Select(translation => new TranslationResponse(
                    translation.Id,
                    translation.Term,
                    translation.Language,
                    translation.Category))
                .ToList()
        );
    }
}