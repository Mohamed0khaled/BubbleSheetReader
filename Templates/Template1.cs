using BubbleSheetReader.Models;

namespace BubbleSheetReader.Templates;

public static class Template1
{
    public static BubbleSheetTemplate Create()
    {
        var template = new BubbleSheetTemplate
        {
            Name = "Template 1",
            ReferenceImageSize = new TemplateImageSize { Width = 2481, Height = 3509 }
        };

        // These coordinates were measured from TestData/dotted_only.jpg and
        // TestData/empty.jpg. They describe the center of each printed bubble.
        template.IdDigitColumns = CreateIdDigitColumns();
        template.TruckDigitColumns = CreateTruckDigitColumns();
        template.HeaderGroups["Shift"] = CreateGroup("Shift", new[] { "Day", "Night" }, new[] { (991, 583), (1275, 583) });
        template.HeaderGroups["Crew"] = CreateGroup("Crew", new[] { "1", "2", "3" }, new[] { (1091, 678), (1179, 678), (1267, 678) });

        for (var rowNumber = 1; rowNumber <= 30; rowNumber++)
        {
            template.DailyRows.Add(CreateDailyRow(rowNumber));
        }

        return template;
    }

    private static List<BubbleGroup> CreateTruckDigitColumns()
    {
        var columns = CreateDigitColumns("Truck No", 2);
        var xCenters = new[] { 533, 627 };
        var yCenters = new[] { 239, 301, 364, 426, 488, 550, 613, 675, 738, 800 };
        ConfigureDigitColumns(columns, xCenters, yCenters);
        return columns;
    }

    private static DailyRowTemplate CreateDailyRow(int rowNumber)
    {
        var row = new DailyRowTemplate();
        var yCenters = new[] { 1047, 1117, 1186, 1255, 1324, 1463, 1532, 1601, 1670, 1739, 1878, 1947, 2017, 2086, 2156, 2295, 2364, 2434, 2503, 2573, 2712, 2782, 2851, 2921, 2990, 3129, 3199, 3268, 3338, 3407 };
        var y = yCenters[rowNumber - 1];

        ConfigureGroup(row.Equipment, new[] { 157, 238, 320, 401, 483 }, y);
        ConfigureGroup(row.Equipment, new[] { 644, 726, 807, 889 }, y, 5);
        ConfigureGroup(row.Equipment, new[] { 1050, 1132, 1214, 1295 }, y, 9);
        ConfigureGroup(row.Material, new[] { 1539, 1700, 1780, 1862, 1943 }, y);
        return row;
    }

    private static BubbleGroup CreateGroup(string name, string[] values, (int X, int Y)[] centers)
    {
        var group = new BubbleGroup(name, values);
        for (var index = 0; index < centers.Length; index++)
        {
            group.Options[index].Region = CreateRegion(centers[index].X, centers[index].Y);
        }
        return group;
    }

    private static void ConfigureGroup(BubbleGroup group, int[] xCenters, int y, int optionStart = 0)
    {
        for (var index = 0; index < xCenters.Length; index++)
        {
            group.Options[optionStart + index].Region = CreateRegion(xCenters[index], y);
        }
    }

    private static void ConfigureDigitColumns(List<BubbleGroup> columns, int[] xCenters, int[] yCenters)
    {
        for (var column = 0; column < columns.Count; column++)
        {
            for (var digit = 0; digit < yCenters.Length; digit++)
            {
                columns[column].Options[digit].Region = CreateRegion(xCenters[column], yCenters[digit]);
            }
        }
    }

    private static BubbleRegion CreateRegion(int x, int y)
    {
        return new BubbleRegion { X = x - 17, Y = y - 17, Width = 34, Height = 34 };
    }

    private static List<BubbleGroup> CreateIdDigitColumns()
    {
        var columns = new List<BubbleGroup>();
        var xCenters = new[] { 97, 188, 279, 370 };
        var yCenters = new[] { 232, 294, 356, 419, 482, 544, 606, 669, 731, 793 };

        for (var column = 0; column < xCenters.Length; column++)
        {
            var group = new BubbleGroup($"ID digit {column + 1}", Enumerable.Range(0, 10).Select(value => value.ToString()));
            for (var digit = 0; digit < yCenters.Length; digit++)
            {
                group.Options[digit].Region = CreateRegion(xCenters[column], yCenters[digit]);
            }
            columns.Add(group);
        }

        return columns;
    }

    private static List<BubbleGroup> CreateDigitColumns(string name, int columnCount)
    {
        var columns = new List<BubbleGroup>();
        for (var column = 1; column <= columnCount; column++)
        {
            columns.Add(new BubbleGroup($"{name} digit {column}", Enumerable.Range(0, 10).Select(value => value.ToString())));
        }
        return columns;
    }
}
