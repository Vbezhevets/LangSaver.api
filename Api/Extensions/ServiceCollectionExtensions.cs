using System.Text;
using LangSaver.Application.Interfaces;
using LangSaver.Application.Services;
using LangSaver.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Langsaver.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLangSaverDatabase(
        this IServiceCollection services, //collecion of services builder.services
        IConfiguration configuration) //from settings, env, args, etc
    {
        services.AddDbContext<LangSaverDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
    public static IServiceCollection AddLangSaverServices(this IServiceCollection services)
    {
        services.AddAuthorization(); //регистрируем стандартный сервис -лайфтайм уже внутри потому что стандартный сервис от asp
        services.AddScoped<IWordService, WordService>();
        services.AddScoped<ITranslatorService, TranslatorService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddSingleton<JwtService>(); // создается раз содержит только конфигурацию

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings")); //ready settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!; //читает из settings.json и мэпит в объект - по сути получаем нужный объект из настройек
    
    services
        .AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options => // creating rules what to check
        {
            options.TokenValidationParameters = new ()
            {
                ValidateIssuer = true, 
                ValidateAudience = false, //
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });
        return services;
    }

}