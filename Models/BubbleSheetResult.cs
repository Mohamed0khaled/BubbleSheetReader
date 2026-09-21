namespace BubbleSheetReader.Models;

public class BubbleSheetResult
{
    public string SourceFileName { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string Id { get; set; } = "Not detected";
    public string TruckNo { get; set; } = "Not detected";
    public string Name { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
    public string Crew { get; set; } = string.Empty;
    public string StartHours { get; set; } = string.Empty;
    public string FinishHours { get; set; } = string.Empty;
    public bool HandwritingIsStub { get; set; }
    public string? DebugImageUrl { get; set; }
    public List<DailyLoadRow> DailyRows { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
