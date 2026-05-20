using LangSaver.Api;


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






 