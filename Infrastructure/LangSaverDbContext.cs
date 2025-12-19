using Microsoft.EntityFrameworkCore;
using LangSaver.Domain;
public class LangSaverDbContext : DbContext
{
    public LangSaverDbContext( DbContextOptions<LangSaverDbContext> options) : base (options){}

    public DbSet<Word> Words {get; set; } 
    public DbSet<User> Users {get; set; } 
}


