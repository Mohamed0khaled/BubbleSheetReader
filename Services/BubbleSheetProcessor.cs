using BubbleSheetReader.Models;
using BubbleSheetReader.Templates;

namespace BubbleSheetReader.Services;

public class BubbleSheetProcessor
{
    private readonly ImageProcessingService imageProcessingService;
    private readonly BubbleDetectionService bubbleDetectionService;
    private readonly IHandwritingService handwritingService;
    private readonly IWebHostEnvironment environment;

    public BubbleSheetProcessor(ImageProcessingService imageProcessingService, BubbleDetectionService bubbleDetectionService, IHandwritingService handwritingService, IWebHostEnvironment environment)
    {
        this.imageProcessingService = imageProcessingService;
        this.bubbleDetectionService = bubbleDetectionService;
        this.handwritingService = handwritingService;
        this.environment = environment;
    }

    public async Task<BubbleSheetResult> ProcessAsync(IFormFile file)
    {
        var template = Template1.Create();
        var result = new BubbleSheetResult
        {
            SourceFileName = file.FileName,
            TemplateName = template.Name,
            HandwritingIsStub = true
        };

        using var prepared = await imageProcessingService.PrepareAsync(file);
        result.Warnings.AddRange(prepared.Warnings);
        var detections = AddHeaderResults(result, template, prepared.Image);
        AddHandwritingResults(result);
        AddDailyRows(result, template, prepared.Image, detections);
        if (prepared.Image is not null)
        {
            var debugFileName = $"id-debug-{Guid.NewGuid():N}.jpg";
            var debugDirectory = Path.Combine(environment.WebRootPath, "debug");
            Directory.CreateDirectory(debugDirectory);
            await bubbleDetectionService.SaveDebugImageAsync(prepared.Image, detections, Path.Combine(debugDirectory, debugFileName));
            result.DebugImageUrl = $"/debug/{debugFileName}";
        }

        result.Warnings.Add("Handwriting recognition is currently using fixed test values. These values were not read from the uploaded document.");
        return result;
    }

    private List<BubbleDetectionResult> AddHeaderResults(BubbleSheetResult result, BubbleSheetTemplate template, SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>? image)
    {
        var detections = new List<BubbleDetectionResult>();
        result.Id = DetectDigits(result, template.IdDigitColumns, image, detections);
        result.TruckNo = DetectDigits(result, template.TruckDigitColumns, image, detections);
        result.Shift = DetectGroup(result, template.HeaderGroups["Shift"], image, detections);
        result.Crew = DetectGroup(result, template.HeaderGroups["Crew"], image, detections);
        return detections;
    }

    private string DetectDigits(BubbleSheetResult result, List<BubbleGroup> columns, SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>? image, List<BubbleDetectionResult> detections)
    {
        var digits = new List<string>();
        foreach (var column in columns)
        {
            var detection = bubbleDetectionService.Detect(image, column, Template1.Create().ReferenceImageSize);
            detections.Add(detection);
            if (!detection.IsValid)
            {
                result.Warnings.Add($"{column.Name}: {detection.Message}");
                continue;
            }
            digits.Add(detection.SelectedValue!);
        }
        return digits.Count == columns.Count ? string.Join(string.Empty, digits) : "Not detected";
    }

    private string DetectGroup(BubbleSheetResult result, BubbleGroup group, SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>? image, List<BubbleDetectionResult> detections)
    {
        var detection = bubbleDetectionService.Detect(image, group, Template1.Create().ReferenceImageSize);
        detections.Add(detection);
        if (!detection.IsValid)
        {
            result.Warnings.Add($"{group.Name}: {detection.Message}");
        }
        return detection.SelectedValue ?? "Not detected";
    }

    private void AddHandwritingResults(BubbleSheetResult result)
    {
        var handwriting = handwritingService.ReadHeaderFields();
        result.Name = handwriting.Name;
        result.Date = handwriting.Date;
        result.StartHours = handwriting.StartHours;
        result.FinishHours = handwriting.FinishHours;
    }

    private void AddDailyRows(BubbleSheetResult result, BubbleSheetTemplate template, SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>? image, List<BubbleDetectionResult> detections)
    {
        for (var index = 0; index < template.DailyRows.Count; index++)
        {
            var rowNumber = index + 1;
            var rowTemplate = template.DailyRows[index];
            var equipment = bubbleDetectionService.Detect(image, rowTemplate.Equipment, template.ReferenceImageSize);
            var material = bubbleDetectionService.Detect(image, rowTemplate.Material, template.ReferenceImageSize);
            detections.Add(equipment);
            detections.Add(material);
            var row = new DailyLoadRow { RowNumber = rowNumber };
            row.Equipment = string.Join(", ", equipment.SelectedValues);
            row.Material = string.Join(", ", material.SelectedValues);

            if (!equipment.IsConfigured || !material.IsConfigured)
            {
                row.Status = "Review";
                row.Warnings.Add("Bubble coordinates are not configured for this row.");
            }
            else if (!equipment.IsValid && !material.IsValid && equipment.Message == "No bubble selected." && material.Message == "No bubble selected.")
            {
                row.Status = "Empty";
            }
            else if (equipment.IsValid && material.IsValid)
            {
                var handwriting = handwritingService.ReadDailyFields(rowNumber);
                row.Payload = handwriting.Payload;
                row.Dump = handwriting.Dump;
                row.Time = handwriting.Time;
                row.Status = "OK";
            }
            else
            {
                row.Status = "Review";
                row.Warnings.Add($"Equipment: {equipment.Message}");
                row.Warnings.Add($"Material: {material.Message}");
            }

            foreach (var warning in row.Warnings)
            {
                result.Warnings.Add($"Row {rowNumber}: {warning}");
            }

            result.DailyRows.Add(row);
        }
    }
}
