using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BubbleSheetReader.Services;

public class PreparedImage : IDisposable
{
    public Image<Rgba32>? Image { get; set; }
    public List<string> Warnings { get; } = new();

    public void Dispose()
    {
        Image?.Dispose();
    }
}

public class ImageProcessingService
{
    private static readonly string[] SupportedExtensions = [".jpg", ".jpeg", ".png", ".pdf"];

    public async Task<PreparedImage> PrepareAsync(IFormFile file)
    {
        var prepared = new PreparedImage();
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!SupportedExtensions.Contains(extension))
        {
            prepared.Warnings.Add("Unsupported file type. Use JPG, JPEG, PNG, or PDF.");
            return prepared;
        }

        if (extension == ".pdf")
        {
            prepared.Warnings.Add("PDF was accepted, but PDF-to-image conversion is not configured yet. Upload a PNG or JPG for bubble detection.");
            return prepared;
        }

        await using var stream = file.OpenReadStream();
        prepared.Image = await Image.LoadAsync<Rgba32>(stream);
        return prepared;
    }
}
