using LangSaver.Application.DTO;
using LangSaver.Application.Interfaces;
using LangSaver.Domain;
using Microsoft.EntityFrameworkCore;
using LangSaver.Application.Exceptions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Text.RegularExpressions;

namespace LangSaver.Application.Services;

public class WordService : IWordService
{
    private readonly LangSaverDbContext _db; // рид онли только для ссылки dbcontext это едеинца работы 

public WordService(LangSaverDbContext db)
{
    _db = db;
}
private Word CreateWord(Guid userId, IWordLookupRequest request)
{
    return new Word
    {
        UserId = userId,
        Term = request.Term,
        Language =  request.Language,
        Category =  request.Category
    };
}
private record Lookup(string Term, string Language, string? Category) : IWordLookupRequest;
private Word CreateWord(Word word)
{
    return CreateWord(word.UserId, new Lookup(word.Term, word.Language, word.Category) );
}

private async Task<Word?> FindExistingWord(Guid userId, IWordLookupRequest request)
{
    return await _db.Words.FirstOrDefaultAsync(w =>
        w.UserId == userId &&
        w.Language == request.Language &&
        w.Term.ToLower() == request.Term.ToLower() &&
        w.Category == request.Category
    );
}

private async Task<Word> FindOrCreateTranslatedWord(Guid userId, IWordLookupRequest request)
{
    var word = await FindExistingWord(userId, request);
    if (word != null)
        return word;

    word = CreateWord(userId, request);

    await _db.Words.AddAsync(word);
    return word;
}
public async Task<Word> CreateAsync(Guid userId, WordCreateRequest request)
{
    var existingWord = await FindExistingWord(userId, request);

    if (existingWord != null)
        throw new Exception($"Word already exists with id {existingWord.Id}");

    var translatedText = await _translator.TranslateAsync(request.Term, request.FromLanguage, request.ToLanguage);

    if (translatedText == null)
        throw new TranslationFailedException("Translation failed");

    var sourceWord = CreateWord(userId, request);

    var translatedWord = await FindOrCreateTranslatedWord(userId, new Lookup(translatedText, request.ToLanguage, request.Category));

    sourceWord.Translations.Add(translatedWord);
    translatedWord.Translations.Add(sourceWord);

    await _db.Words.AddAsync(sourceWord);
    await _db.SaveChangesAsync();

    return sourceWord;
}
    public async Task<Word?> QueryAsync(Guid userId, WordQueryRequest request)
    {
        var word = await FindExistingWord(userId, request);

        if (word == null)
            return null;
        
        var translations = await _db.Words.AsNoTracking()
            .Where(w=> w.Language == request.ToLanguage &&
            w.Translations.Any(t=> t.Id == word.Id)).ToListAsync();

        var res = CreateWord(word);
        res.Translations = translations;
        return res;
        
    }

    public async Task<Word?> GetByIdAsync(Guid userId, Guid id)
    {
        return await _db.Words.AsNoTracking()
                              .FirstOrDefaultAsync(w=> w.UserId == userId && w.Id == id);
    }

    public async Task <bool> DeleteAsync(Guid userId, Guid id)
    {
        var existingWord = await _db.Words
                              .FirstOrDefaultAsync(w=> w.UserId == userId && w.Id == id);  
        if (existingWord == null) {
            throw new NotExistingWordException($"Word does not exist with id {id}");
        }
        _db.Words.Remove(existingWord);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<Word?> PatchAsync(Guid userId, Guid id, WordPatchRequest req)
    {
        var word = await _db.Words.FirstOrDefaultAsync(w =>
            w.UserId == userId &&
            w.Id == id
        );

        if (word == null)
            return null;

        if (req.Category != null)
            word.Category = req.Category;

        await _db.SaveChangesAsync();

        return word;
    }


}
