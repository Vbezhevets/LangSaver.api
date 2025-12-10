namespace LangSaver.Application.DTO;
public class WordUpdateRequest
{
    public string Name {get; set; }
    public string Translation {get; set; }
    public string From {get; set; }
    public string To   {get; set; }
    public string? Category {get; set; }

}