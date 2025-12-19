using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LangSaver.Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using LangSaver.Application.Exceptions;
using LangSaver.Application.Exceptions;
using LangSaver.Application.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LangSaverDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IWordService, WordService>(); 
var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var error = exceptionFeature?.Error;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;


        var problem = new ErrorDTO
        {
            Type = "internal_error",
            Title = "Internal Server Error",
            Detail = error?.Message ?? "Unknown error",
            Status = StatusCodes.Status500InternalServerError
        };

        if (error is TranslationFailedException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            problem = new ErrorDTO
            {
                Type = "translation_failed",
                Title = "Translation failed",
                Detail = error.Message,
                Status = StatusCodes.Status400BadRequest
            };
        }
        else if (error is NotExistingWordException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            problem = new ErrorDTO
            {
                Type = "not_exist",
                Title = "Word does not exist",
                Detail = error.Message,
                Status = StatusCodes.Status404NotFound
            };
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

var words = app.MapGroup("/words");


 

app.Run();



