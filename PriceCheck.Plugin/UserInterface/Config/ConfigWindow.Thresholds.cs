using System;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Thresholds()
    {
        using var tabItem = ImRaii.TabItem(Language.Thresholds);
        if (!tabItem.Success)
            return;

        ImGui.TextUnformatted(Language.MinimumPrice);
        ImGuiComponents.HelpMarker(Language.MinimumPrice_HelpMarker);
        var minPrice = Plugin.Configuration.MinPrice;
        if (ImGui.InputInt("###PriceCheck_MinPrice_Slider", ref minPrice, 500, 500))
        {
            Plugin.Configuration.MinPrice = Math.Abs(minPrice);
            Plugin.SaveConfig();
        }

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.MaxUploadDays);
        ImGuiComponents.HelpMarker(Language.MaxUploadDays_HelpMarker);
        var maxUploadDays = Plugin.Configuration.MaxUploadDays;
        if (ImGui.InputInt("###PriceCheck_MaxUploadDays_Slider", ref maxUploadDays, 5, 5))
        {
            Plugin.Configuration.MaxUploadDays = Math.Abs(maxUploadDays);
            Plugin.SaveConfig();
        }
    }
}
