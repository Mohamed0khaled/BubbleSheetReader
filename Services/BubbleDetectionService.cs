using BubbleSheetReader.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BubbleSheetReader.Services;

public class BubbleDetectionResult
{
    public bool IsConfigured { get; init; }
    public bool IsValid { get; init; }
    public string? SelectedValue { get; init; }
    public List<string> SelectedValues { get; init; } = new();
    public string Message { get; init; } = string.Empty;
    public List<BubbleMeasurement> Measurements { get; init; } = new();
}

public class BubbleMeasurement
{
    public string Value { get; init; } = string.Empty;
    public BubbleRegion Region { get; init; } = new();
    public double Darkness { get; init; }
    public string State { get; init; } = string.Empty;
}

public class BubbleDetectionService
{
    public BubbleDetectionResult Detect(Image<Rgba32>? image, BubbleGroup group, TemplateImageSize? referenceImageSize = null)
    {
        if (image is null)
        {
            return NotConfigured("No image is available for bubble detection.");
        }

        if (group.Options.Any(option => !option.Region.IsConfigured))
        {
            return NotConfigured($"{group.Name} coordinates are not configured.");
        }

        var measurements = new List<BubbleMeasurement>();
        var selected = new List<string>();
        foreach (var option in group.Options)
        {
            var region = ScaleRegion(option.Region, image, referenceImageSize);
            var darkness = MeasureDarkPixelRatio(image, region);
            // The blank reference sheet gives printed circles a baseline around
            // 0.24-0.30 dark pixels. Filled bubbles are around 0.65.
            var state = darkness >= 0.45 ? "selected" : darkness >= 0.35 ? "unclear" : "empty";
            measurements.Add(new BubbleMeasurement { Value = option.Value, Region = region, Darkness = darkness, State = state });
            if (state == "selected")
            {
                selected.Add(option.Value);
            }
        }

        if (selected.Count == 1)
        {
            return new BubbleDetectionResult { IsConfigured = true, IsValid = true, SelectedValue = selected[0], SelectedValues = selected, Message = "One bubble selected.", Measurements = measurements };
        }

        if (selected.Count == 0)
        {
            var unclear = measurements.Any(measurement => measurement.State == "unclear");
            return new BubbleDetectionResult { IsConfigured = true, Message = unclear ? "Selection is unclear." : "No bubble selected.", Measurements = measurements };
        }

        return new BubbleDetectionResult { IsConfigured = true, SelectedValues = selected, Message = "Multiple bubbles selected.", Measurements = measurements };
    }

    public async Task SaveDebugImageAsync(Image<Rgba32> image, IEnumerable<BubbleDetectionResult> detections, string path)
    {
        using var debugImage = image.Clone();
        foreach (var detection in detections)
        {
            foreach (var measurement in detection.Measurements)
            {
                var color = measurement.State switch
                {
                    "selected" => new Rgba32(0, 180, 70),
                    "unclear" => new Rgba32(255, 180, 0),
                    _ => new Rgba32(220, 40, 40)
                };
                DrawRectangle(debugImage, measurement.Region, color);
            }
        }

        await debugImage.SaveAsJpegAsync(path);
    }

    private static BubbleDetectionResult NotConfigured(string message)
    {
        return new BubbleDetectionResult { Message = message };
    }

    private static double MeasureDarkPixelRatio(Image<Rgba32> image, BubbleRegion region)
    {
        var darkPixels = 0;
        var totalPixels = 0;
        var right = Math.Min(region.X + region.Width, image.Width);
        var bottom = Math.Min(region.Y + region.Height, image.Height);

        for (var y = Math.Max(0, region.Y); y < bottom; y++)
        {
            for (var x = Math.Max(0, region.X); x < right; x++)
            {
                var pixel = image[x, y];
                var brightness = (pixel.R + pixel.G + pixel.B) / 3;
                if (brightness < 120)
                {
                    darkPixels++;
                }
                totalPixels++;
            }
        }

        return totalPixels == 0 ? 0 : (double)darkPixels / totalPixels;
    }

    private static BubbleRegion ScaleRegion(BubbleRegion region, Image<Rgba32> image, TemplateImageSize? referenceImageSize)
    {
        if (referenceImageSize is null || referenceImageSize.Width <= 0 || referenceImageSize.Height <= 0)
        {
            return region;
        }

        var xScale = (double)image.Width / referenceImageSize.Width;
        var yScale = (double)image.Height / referenceImageSize.Height;
        return new BubbleRegion
        {
            X = (int)Math.Round(region.X * xScale),
            Y = (int)Math.Round(region.Y * yScale),
            Width = Math.Max(1, (int)Math.Round(region.Width * xScale)),
            Height = Math.Max(1, (int)Math.Round(region.Height * yScale))
        };
    }

    private static void DrawRectangle(Image<Rgba32> image, BubbleRegion region, Rgba32 color)
    {
        var left = Math.Max(0, region.X);
        var top = Math.Max(0, region.Y);
        var right = Math.Min(image.Width - 1, region.X + region.Width);
        var bottom = Math.Min(image.Height - 1, region.Y + region.Height);

        for (var x = left; x <= right; x++)
        {
            image[x, top] = color;
            image[x, bottom] = color;
        }
        for (var y = top; y <= bottom; y++)
        {
            image[left, y] = color;
            image[right, y] = color;
        }
    }
}
