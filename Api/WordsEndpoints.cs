using LangSaver.Application.DTO;
using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;


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

 
        words.MapGet("/", async (
            [FromServices] IWordService service,
            [FromQuery] string? term,
            [FromQuery] string? fromLanguage,
            [FromQuery] string? toLanguage,
            [FromQuery] string? category,
            HttpContext context) =>
        {
            if (string.IsNullOrWhiteSpace(term))
                return Results.BadRequest(new { message = "Term is required." });

            if (string.IsNullOrWhiteSpace(fromLanguage))
                return Results.BadRequest(new { message = "From language is required." });

            if (string.IsNullOrWhiteSpace(toLanguage))
                return Results.BadRequest(new { message = "To language is required." });

            var userId = context.GetUserId();

            var req = new WordQueryRequest(
                term.Trim(),
                fromLanguage.Trim().ToLowerInvariant(),
                toLanguage.Trim().ToLowerInvariant(),
                category?.Trim()
            );

            var word = await service.QueryAsync(userId, req);

            return word != null
                ? Results.Ok(word)
                : Results.NotFound();
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

        words.MapGet("/export/csv", async (
            [FromServices] IWordService service,
            [FromQuery] string? language,
            HttpContext context) =>
        {
            if (string.IsNullOrWhiteSpace(language))
                return Results.BadRequest(new { message = "Language query parameter is required." });

            var userId = context.GetUserId();

            var csv = await service.ExportCsvAsync(userId, language);
            var bytes = Encoding.UTF8.GetBytes(csv);

            return Results.File(
                bytes,
                contentType: "text/csv",
                fileDownloadName: $"langsaver-words-{language}.csv"
            );
        });
        return app;

    }
    
}

