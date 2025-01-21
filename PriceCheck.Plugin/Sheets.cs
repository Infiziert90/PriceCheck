using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace PriceCheck;

public static class Sheets
{
    public static readonly ExcelSheet<Item> ItemSheet;
    public static readonly ExcelSheet<ContentFinderCondition> ContentFinderSheet;

    static Sheets()
    {
        ItemSheet = Plugin.DataManager.GetExcelSheet<Item>();
        ContentFinderSheet = Plugin.DataManager.GetExcelSheet<ContentFinderCondition>();
    }
}
