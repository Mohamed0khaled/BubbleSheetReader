namespace BubbleSheetReader.Services;

public interface IHandwritingService
{
    HandwritingValues ReadHeaderFields();
    HandwritingValues ReadDailyFields(int rowNumber);
}

public class HandwritingValues
{
    public string Name { get; init; } = string.Empty;
    public string Date { get; init; } = string.Empty;
    public string StartHours { get; init; } = string.Empty;
    public string FinishHours { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public string Dump { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
}

public class HandwritingStubService : IHandwritingService
{
    public HandwritingValues ReadHeaderFields()
    {
        return new HandwritingValues
        {
            Name = "TEST NAME",
            Date = "2026-09-22",
            StartHours = "1000",
            FinishHours = "1100"
        };
    }

    public HandwritingValues ReadDailyFields(int rowNumber)
    {
        return new HandwritingValues
        {
            Payload = "50",
            Dump = "TEST DUMP",
            Time = "30"
        };
    }
}
