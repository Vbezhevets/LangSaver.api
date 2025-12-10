namespace LangSaver.Domain;
public class User
{
    public Guid Id {get; set; }

    public string Name {get; init; }

    public string  eMail {get; set; };

    public List <Word> Words { get; set; } = []; 
}