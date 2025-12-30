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

        if (!email.Contains('@') || password.Length < 6)
            return Results.BadRequest(new { message = "Invalid email or password too short" });

            var exists = await db.Users.AnyAsync(u => u.Email == email);
            if (exists)
                return Results.Conflict(new { message = "User already exists" });

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