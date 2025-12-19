using LangSaver.Application.DTO;
using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LangSaver.Api;

public static class WordsEndpoints
{

    public static IEndpointRouteBuilder MapWordsEndpoints(this IEndpointRouteBuilder app)
    {
        var words = app.MapGroup("/words");

        words.MapPost("/", async ( IWordService service, WordCreateRequest req) =>
        {
            Guid userId = null;
            var w = await service.CreateAsync(userId, req);

            return Results.Created($"/words/{w.Id}", w);
        });

 
        words.MapGet("/", async ( IWordService service, WordQueryRequest req) =>
        {
            Guid userId = null;

            var w = await service.QueryAsync(userId, req);

            return w != null ? Results.Ok(w) : Results.NotFound();
        });

        words.MapGet("/{id}", async (IWordService service, Guid id) =>
        {
            Guid userId = Guid.Empty; // временно

            var word = await service.GetByIdAsync(userId, id);

            return word != null
                ? Results.Ok(word)
                : Results.NotFound();
        });

        words.MapPut("/", async (IWordService service, Guid id, WordPatchRequest req) =>
        {
            Guid userId = Guid.Empty;
            var word = await service.PatchAsync(userId, id, req);
            return word != null
                ? Results.Ok(word)
                : Results.NotFound();

        });

        words.MapDelete("/", async ( IWordService service, Guid id) =>
        {
            Guid userId = null;
            return  await service.DeleteAsync(userId, id);
        });
    }
}

