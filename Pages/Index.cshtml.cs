using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using BubbleSheetReader.Services;

namespace BubbleSheetReader.Pages;

public class IndexModel : PageModel
{
    private readonly BubbleSheetProcessor processor;

    public IndexModel(BubbleSheetProcessor processor)
    {
        this.processor = processor;
    }

    [BindProperty]
    public IFormFile? Upload { get; set; }

    [BindProperty]
    public string Template { get; set; } = "Template 1";

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Upload is null || Upload.Length == 0)
        {
            ErrorMessage = "Choose a JPG, JPEG, PNG, or PDF file first.";
            return Page();
        }

        var result = await processor.ProcessAsync(Upload);
        HttpContext.Session.SetString("BubbleSheetResult", JsonSerializer.Serialize(result));
        return RedirectToPage("/Result");
    }
}
