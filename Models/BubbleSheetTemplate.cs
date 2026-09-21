namespace BubbleSheetReader.Models;

public class BubbleRegion
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public bool IsConfigured => Width > 0 && Height > 0;
}

public class TemplateImageSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class BubbleOption
{
    public BubbleOption(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public BubbleRegion Region { get; set; } = new();
}

public class BubbleGroup
{
    public BubbleGroup(string name, IEnumerable<string> values)
    {
        Name = name;
        Options = values.Select(value => new BubbleOption(value)).ToList();
    }

    public string Name { get; }
    public List<BubbleOption> Options { get; }
}

public class DailyRowTemplate
{
    public DailyRowTemplate()
    {
        Equipment = new BubbleGroup("Equipment", new[] { "EX05", "EX08", "EX09", "EX11", "EX12", "SH03", "SH04", "SH05", "SH06", "L01", "L02", "L03", "L04" });
        Material = new BubbleGroup("Material", new[] { "W", "S", "L", "M", "H" });
    }

    public BubbleGroup Equipment { get; }
    public BubbleGroup Material { get; }
}

public class BubbleSheetTemplate
{
    public string Name { get; set; } = string.Empty;
    public TemplateImageSize ReferenceImageSize { get; set; } = new();
    public Dictionary<string, BubbleGroup> HeaderGroups { get; set; } = new();
    public List<BubbleGroup> IdDigitColumns { get; set; } = new();
    public List<BubbleGroup> TruckDigitColumns { get; set; } = new();
    public List<DailyRowTemplate> DailyRows { get; set; } = new();
}
