namespace LangSaver.Application.DTO;
public class WordQueryRequest
{
    
    public string Name {get; set; }
    public string From {get; set; }
    public string To   {get; set; }
    public string? Category {get; set; }

}