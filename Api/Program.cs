using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LangSaver.Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using LangSaver.Application.Exceptions;
using LangSaver.Application.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LangSaver.Domain;
using LangSaver.Api;
using Microsoft.EntityFrameworkCore.Query.Internal;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LangSaverDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); //calculated
builder.Services.AddAuthorization(); //регистрируем стандартный сервис -лайфтайм уже внутри потому что стандартный сервис от asp
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<ITranslatorService, TranslatorService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings")); //ready settings
builder.Services.AddSingleton<JwtService>(); // создается раз содержит только конфигурацию
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!; //читает из settings.json и мэпит в объект - по сути получаем нужный объект из настройек?
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => // creating rules what to check
    {
        options.TokenValidationParameters = new ()
        {
            ValidateIssuer = true, 
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
var app = builder.Build();
//место для middlaware:

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
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
app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapAuthEndpoints(); 
app.MapWordsEndpoints();


app.Run();






 