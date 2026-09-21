using BubbleSheetReader.Models;
using ClosedXML.Excel;

namespace BubbleSheetReader.Services;

public class ExcelService
{
    private static readonly string[] ColumnNames =
    [
        "Name", "Crew", "Shift", "SMU Start", "SMU End", "Dump", "Payload", "DT", "Image of Name",
        "EX5", "EX8", "EX9", "EX11", "EX012", "SH3", "SH4", "SH5", "SH6", "L1", "L2", "L3", "L4",
        "Ma-W", "Ma-H", "Ma-M", "Ma-L", "Ma-S", "Time"
    ];

    public byte[] CreateWorkbook(BubbleSheetResult result)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Daily Load");

        for (var column = 0; column < ColumnNames.Length; column++)
        {
            worksheet.Cell(1, column + 1).Value = ColumnNames[column];
        }

        var excelRow = 2;
        foreach (var row in result.DailyRows.Where(row => row.IsPopulated))
        {
            var values = CreateRowValues(result, row);
            for (var column = 0; column < values.Length; column++)
            {
                worksheet.Cell(excelRow, column + 1).Value = values[column];
            }
            excelRow++;
        }

        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string[] CreateRowValues(BubbleSheetResult result, DailyLoadRow row)
    {
        return
        [
            result.Name, result.Crew, result.Shift, result.StartHours, result.FinishHours,
            row.Dump, row.Payload, result.Date, "Name image placeholder", IsSelected(row.Equipment, "EX05"),
            IsSelected(row.Equipment, "EX08"), IsSelected(row.Equipment, "EX09"), IsSelected(row.Equipment, "EX11"),
            IsSelected(row.Equipment, "EX12"), IsSelected(row.Equipment, "SH03"), IsSelected(row.Equipment, "SH04"),
            IsSelected(row.Equipment, "SH05"), IsSelected(row.Equipment, "SH06"), IsSelected(row.Equipment, "L01"),
            IsSelected(row.Equipment, "L02"), IsSelected(row.Equipment, "L03"), IsSelected(row.Equipment, "L04"),
            IsSelected(row.Material, "W"), IsSelected(row.Material, "H"), IsSelected(row.Material, "M"),
            IsSelected(row.Material, "L"), IsSelected(row.Material, "S"), row.Time
        ];
    }

    private static string IsSelected(string actualValue, string expectedValue)
    {
        return actualValue == expectedValue ? "1" : string.Empty;
    }
}
