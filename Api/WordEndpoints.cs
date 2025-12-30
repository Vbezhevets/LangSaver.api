using System.Runtime.InteropServices;
using LangSaver.Application.DTO;
using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace LangSaver.Api;

public static class WordsEndpoints
{

    public static IEndpointRouteBuilder MapWordsEndpoints(this IEndpointRouteBuilder app)
    {
        var words = app.MapGroup("/words").RequireAuthorization();

        words.MapPost("/", async ([FromServices] IWordService service, [FromBody] WordCreateRequest req, HttpContext context) =>
        {
            Guid userId = context.GetUserId(); 
            var w = await service.CreateAsync(userId, req);

            return Results.Created($"/words/{w.Id}", w);
        });

 
        words.MapGet("/", async ([FromServices]  IWordService service,[FromBody] WordQueryRequest req, HttpContext context) =>
        {
            Guid userId = context.GetUserId(); 

            var w = await service.QueryAsync(userId, req);

            return w != null ? Results.Ok(w) : Results.NotFound();
        });

        words.MapGet("/{id}", async (IWordService service, Guid id, HttpContext context) =>
        {
            Guid userId = context.GetUserId(); 

            var word = await service.GetByIdAsync(userId, id);

            return word != null
                ? Results.Ok(word)
                : Results.NotFound();
        });

        words.MapPatch("/{id}", async ([FromServices] IWordService service, Guid id,[FromBody] WordPatchRequest req, HttpContext context) =>
        {
            Guid userId = context.GetUserId(); 
            var word = await service.PatchAsync(userId, id, req);
            return word != null
                ? Results.Ok(word)
                : Results.NotFound();

        });

        words.MapDelete("/{id}", async (IWordService service, Guid id, HttpContext context) =>
        {
            Guid userId = context.GetUserId(); 
            bool res = await service.DeleteAsync(userId, id);
            return  res ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}

