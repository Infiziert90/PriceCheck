using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Filters()
    {
        using var tabItem = ImRaii.TabItem(Language.Filters);
        if (!tabItem.Success)
            return;

        var restrictInCombat = Plugin.Configuration.RestrictInCombat;
        if (ImGui.Checkbox(Language.RestrictInCombat, ref restrictInCombat))
        {
            Plugin.Configuration.RestrictInCombat = restrictInCombat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.RestrictInCombat_HelpMarker);

        var restrictInContent = Plugin.Configuration.RestrictInContent;
        if (ImGui.Checkbox(Language.RestrictInContent, ref restrictInContent))
        {
            Plugin.Configuration.RestrictInContent = restrictInContent;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.RestrictInContent_HelpMarker);
    }
}
