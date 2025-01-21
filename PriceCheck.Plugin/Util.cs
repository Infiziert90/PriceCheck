using Lumina.Extensions;

namespace PriceCheck;

public static class Util
{
    public static bool CheckContent(uint territoryId)
    {
        var result = Sheets.ContentFinderSheet.FirstOrNull(c => c.TerritoryType.RowId == territoryId);
        return result != null;
    }
}
