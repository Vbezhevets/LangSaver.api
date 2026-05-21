using LangSaver.Application.DTO;
using LangSaver.Domain;

namespace LangSaver.Application.Interfaces;

public  interface IWordService
{
    Task<WordResponse> CreateAsync(Guid userId, WordCreateRequest req);
    Task<WordResponse?> QueryAsync(Guid userId, WordQueryRequest req);
    Task<WordResponse?> GetByIdAsync(Guid userId, Guid id);

    Task <WordResponse?> PatchAsync(Guid userId, Guid id, WordPatchRequest req);
    Task <bool> DeleteAsync(Guid userId, Guid id);
    Task<string> ExportCsvAsync(Guid userId, string language);


}