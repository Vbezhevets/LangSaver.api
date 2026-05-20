using LangSaver.Application.DTO;
using LangSaver.Domain;
using LangSaver.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace LangSaver.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/auth");

        auth.MapPost("/register", async (
            [FromBody] RegisterRequest req,
            [FromServices] LangSaverDbContext db,
            [FromServices] IPasswordHasher<User> hasher,
            [FromServices] JwtService jwt) =>
        {
            var email = (req.Email ?? "").Trim().ToLowerInvariant();
            var password = req.Password ?? "";

            if (string.IsNullOrWhiteSpace(email))
                return Results.BadRequest(new { message = "Email is required." });

            if (!email.Contains('@') || !email.Contains('.'))
                return Results.BadRequest(new { message = "Email must be a valid email address." });

            if (email.Length > 255)
                return Results.BadRequest(new { message = "Email must not exceed 255 characters." });

            if (email.Any(char.IsWhiteSpace))
                return Results.BadRequest(new { message = "Email must not contain whitespace." });

            if (string.IsNullOrWhiteSpace(password))
                return Results.BadRequest(new { message = "Password is required." });

            if (password.Length < 8)
                return Results.BadRequest(new { message = "Password must be at least 8 characters long." });

            if (password.Length > 100)
                return Results.BadRequest(new { message = "Password must not exceed 100 characters." });

            if (!password.Any(char.IsLetter))
                return Results.BadRequest(new { message = "Password must contain at least one letter." });

            if (!password.Any(char.IsDigit))
                return Results.BadRequest(new { message = "Password must contain at least one digit." });

            var exists = await db.Users.AnyAsync(u => u.Email == email);
            if (exists)
                return Results.Conflict(new { message = "User already exists." });

            var user = new User { Email = email };
            user.PasswordHash = hasher.HashPassword(user, password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = jwt.GenerateAccessToken(user.Id);

            return Results.Ok(new { userId = user.Id, accessToken = token });
        });

        auth.MapPost("/login", async (
            [FromBody] LoginRequest req,
            [FromServices] LangSaverDbContext db,
            [FromServices] IPasswordHasher<User> hasher,
            [FromServices] JwtService jwt) =>
        {
            var email = (req.Email ?? "").Trim().ToLowerInvariant();
            var password = req.Password ?? "";

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Results.Unauthorized();

            var res = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (res == PasswordVerificationResult.Failed)
                return Results.Unauthorized();

            var token = jwt.GenerateAccessToken(user.Id);
            return Results.Ok(new { userId = user.Id, accessToken = token });
        });

        return app;
    }
}