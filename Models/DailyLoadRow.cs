namespace BubbleSheetReader.Models;

public class DailyLoadRow
{
    public int RowNumber { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Dump { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Status { get; set; } = "Empty";
    public List<string> Warnings { get; set; } = new();
    public bool IsPopulated => Status == "OK";
}
