using Microsoft.EntityFrameworkCore;

public class LangSaverDbContext : DbContext
{
    public LangSaverDbContext( DbContextOptions<LangSaverDbContext> options) : base (options){}

    public DbSet<Word> Words => Set<Word>(); 
    public DbSet<User> Users =>   Set<User>();
}


