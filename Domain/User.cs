namespace LangSaver.Domain;
public class User
{
    public Guid Id {get; set; } = Guid.NewGuid();

    public string Email {get; set; } = null!;
    public string? PasswordHash {get; set; } // selbst anmeldet
    public string? GoogleId {get; set; }
    public string? RefreshToken {get; set; } // Aktualisierung ohne Anmeldung

    public DateTime? RefreshTokenExpiresAt {get; set;}
    public DateTime CreatedAt {get; set;}


}