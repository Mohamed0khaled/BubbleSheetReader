using System.Text.Json;
using BubbleSheetReader.Models;
using BubbleSheetReader.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BubbleSheetReader.Pages;

public class ResultModel : PageModel
{
    private readonly ExcelService excelService;

    public ResultModel(ExcelService excelService)
    {
        this.excelService = excelService;
    }

    public BubbleSheetResult Result { get; private set; } = new();

    public IActionResult OnGet()
    {
        return LoadResult();
    }

    public IActionResult OnGetDownload()
    {
        var action = LoadResult();
        if (action is not PageResult)
        {
            return action;
        }

        var bytes = excelService.CreateWorkbook(Result);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "bubble-sheet-result.xlsx");
    }

    private IActionResult LoadResult()
    {
        var json = HttpContext.Session.GetString("BubbleSheetResult");
        if (string.IsNullOrWhiteSpace(json))
        {
            return RedirectToPage("/Index");
        }

        Result = JsonSerializer.Deserialize<BubbleSheetResult>(json) ?? new BubbleSheetResult();
        return Page();
    }
}
