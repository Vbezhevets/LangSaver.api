using LangSaver.Application.DTO;
using LangSaver.Domain;

namespace LangSaver.Application.Interfaces;

public  interface IWordService
{
    Task<Word> CreateAsync(Guid userId, WordCreateRequest req);
    Task<Word?> QueryAsync(Guid userId, WordQueryRequest req);
    Task<Word?> GetByIdAsync(Guid userId, Guid id);

    Task <Word?> PatchAsync(Guid userId, Guid id, WordPatchRequest req);
    Task <bool> DeleteAsync(Guid userId, Guid id);


}