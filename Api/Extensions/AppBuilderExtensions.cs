using LangSaver.Application.DTO;
using LangSaver.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LangSaver.Api;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseLangSaverExceptions(this IApplicationBuilder app)
    {
        
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context => //.Run means final code. принимает делегат Task Handle(HttpContext context)
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>(); //contextHPP содержит инфу о запросе middleware туда может добавлять
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
        return app;
    }

}