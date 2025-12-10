using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using LangSaver.Application.DTO;
using Microsoft.AspNetCore.Identity;
using LangSaver.Application.Exceptions;
using LangSaver.Domain;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LangSaverDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (error is TranslationFailedException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(error.Message);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Internal server error");
    });
});


var words = app.MapGroup("/words");

words.MapDelete("/{id}", async (LangSaverDbContext db, Guid user, Guid id) =>
{
    var w = await db.Words.FirstOrDefaultAsync(wo =>
        wo.UserId == user &&
        wo.Id == id);

    if (w is null)
        return Results.NotFound(new { error = "No such object" });

    db.Words.Remove(w);
    await db.SaveChangesAsync();

    return Results.Ok(w);
});


words.MapPatch("/{id}", async (LangSaverDbContext db, Guid user, Guid id, JsonElement prop) =>
{
    var w = await db.Words.FirstOrDefaultAsync(wo =>
        wo.UserId == user &&
        wo.Id == id);

    if (w is null)
        return Results.NotFound(new { error = "No such object" });

    if (prop.TryGetProperty("Name", out var name))
        w.Name = name.GetString();

    if (prop.TryGetProperty("Category", out var cat))
        w.Category = cat.ValueKind == JsonValueKind.Null ? null : cat.GetString();

    if (prop.TryGetProperty("Translation", out var trans))
        w.Translation = trans.GetString();

    if (prop.TryGetProperty("From", out var from))
        w.From = from.GetString();

    if (prop.TryGetProperty("To", out var to))
        w.To = to.GetString();

    await db.SaveChangesAsync();

    return Results.Ok(w);
});


words.MapPut("/{id}", async (LangSaverDbContext db, Guid user, Guid id, WordUpdateRequest req) =>
{
    var w = await db.Words.FirstOrDefaultAsync(wo =>
        wo.Id == id &&
        wo.UserId == user);

    if (w is null)
        return Results.NotFound(new { error = "No such object" });

    w.Name = req.Name;
    w.Translation = req.Translation;
    w.Category = req.Category;
    w.From = req.From;
    w.To = req.To;

    await db.SaveChangesAsync();

    return Results.Ok(w);
});


words.MapPost("/", async (LangSaverDbContext db, Guid user, WordCreateRequest req) =>
{
    var exists = await db.Words.FirstOrDefaultAsync(wo =>
        wo.UserId == user &&
        wo.From == req.From &&
        wo.To == req.To &&
        (
            wo.Category == req.Category ||
            (wo.Category == null && req.Category == null)
        ) &&
        EF.Functions.ILike(wo.Name, req.Name));

    if (exists is not null)
        return Results.Conflict(new { error = "Word exists", id = exists.Id });

    string? translation = null;

    if (translation is null)
        return Results.Problem("Translation failed", statusCode: 502);

    var w = new Word
    {
        UserId = user,
        Name = req.Name,
        Translation = translation,
        Category = req.Category,
        From = req.From,
        To = req.To,
    };

    db.Words.Add(w);
    await db.SaveChangesAsync();

    return Results.Created($"/words/{w.Id}", w);
});

words.MapGet("/", async (LangSaverDbContext db, Guid user, WordQueryRequest req) =>
{
    var query = db.Words.Where(wo =>
        wo.UserId == user &&
        wo.From == req.From &&
        wo.To == req.To &&
        EF.Functions.ILike(wo.Name, req.Name));

    if (req.Category is not null)
        query = query.Where(wo => wo.Category == req.Category);

    var res = await query.FirstOrDefaultAsync();

    if (res is null)
        return Results.NotFound(new { error = "The word not found" });

    return Results.Ok(res);
});

app.Run();