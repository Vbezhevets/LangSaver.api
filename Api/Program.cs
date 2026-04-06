using LangSaver.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using LangSaver.Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using LangSaver.Application.Exceptions;
using LangSaver.Application.DTO;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LangSaver.Domain;
using LangSaver.Api;
using Langsaver.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLangSaverDatabase(builder.Configuration);
builder.Services.AddLangSaverServices();
builder.Services.AddJwtAuthentication(builder.Configuration);


var app = builder.Build();

app.UseLangSaverExceptions();
app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapAuthEndpoints(); 
app.MapWordsEndpoints();


app.Run();






 