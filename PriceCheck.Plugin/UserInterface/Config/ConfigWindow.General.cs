using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void General()
    {
        using var tabItem = ImRaii.TabItem(Language.General);
        if (!tabItem.Success)
            return;

        var enabled = Plugin.Configuration.Enabled;
        if (ImGui.Checkbox(Language.PluginEnabled, ref enabled))
        {
            Plugin.Configuration.Enabled = enabled;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.PluginEnabled_HelpMarker);

        var showPrices = Plugin.Configuration.ShowPrices;
        if (ImGui.Checkbox(Language.ShowPrices, ref showPrices))
        {
            Plugin.Configuration.ShowPrices = showPrices;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowPrices_HelpMarker);

        var allowKeybindAfterHover = Plugin.Configuration.AllowKeybindAfterHover;
        if (ImGui.Checkbox(Language.AllowKeybindAfterHover, ref allowKeybindAfterHover))
        {
            Plugin.Configuration.AllowKeybindAfterHover = allowKeybindAfterHover;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.AllowKeybindAfterHover_HelpMarker);

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.HoverDelay);
        ImGuiComponents.HelpMarker(Language.HoverDelay_HelpMarker);
        var hoverDelay = Plugin.Configuration.HoverDelay;
        if (ImGui.SliderInt("###PriceCheck_HoverDelay_Slider", ref hoverDelay, 0, 10))
        {
            Plugin.Configuration.HoverDelay = hoverDelay;
            Plugin.SaveConfig();
        }

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.PriceMode);
        ImGuiComponents.HelpMarker(Language.PriceMode_HelpMarker);
        var priceMode = Plugin.Configuration.PriceMode;
        if (ImGui.Combo("###PriceCheck_PriceMode_Combo", ref priceMode, PriceMode.PriceModeNames.ToArray(), PriceMode.PriceModeNames.Count))
        {
            Plugin.Configuration.PriceMode = priceMode;
            Plugin.SaveConfig();
        }

        using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
        ImGui.TextWrapped(PriceMode.GetPriceModeByIndex(priceMode)?.Description);

        ImGui.Spacing();
    }
}
