using Microsoft.EntityFrameworkCore;
using LangSaver.Domain;
public class LangSaverDbContext : DbContext
{
    public LangSaverDbContext( DbContextOptions<LangSaverDbContext> options) : base (options){}

    public DbSet<Word> Words {get; set; } 
    public DbSet<User> Users {get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email) // по какому юник полю создавть индекс
            .IsUnique();
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();
    }
}
