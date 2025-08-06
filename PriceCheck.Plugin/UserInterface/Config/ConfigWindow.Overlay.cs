using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Overlay()
    {
        using var tabItem = ImRaii.TabItem(Language.Overlay);
        if (!tabItem.Success)
            return;

        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.DisplayHeading);
        ImGui.Spacing();

        var showOverlay = Plugin.Configuration.ShowOverlay;
        if (ImGui.Checkbox(Language.ShowOverlay, ref showOverlay))
        {
            Plugin.Configuration.ShowOverlay = showOverlay;
            Plugin.MainWindow.IsOpen = showOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowOverlay_HelpMarker);

        var showOverlayOnLogin = Plugin.Configuration.ShowOverlayOnLogin;
        if (ImGui.Checkbox(Language.ShowOverlayOnLogin, ref showOverlayOnLogin))
        {
            Plugin.Configuration.ShowOverlayOnLogin = showOverlayOnLogin;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowOverlayOnLogin_HelpMarker);

        var showOverlayByKeybind = Plugin.Configuration.ShowOverlayByKeybind;
        if (ImGui.Checkbox(Language.ShowOverlayByKeybind, ref showOverlayByKeybind))
        {
            Plugin.Configuration.ShowOverlayByKeybind = showOverlayByKeybind;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowOverlayByKeybind_HelpMarker);

        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.StyleHeading);
        ImGui.Spacing();

        var useOverlayColors = Plugin.Configuration.UseOverlayColors;
        if (ImGui.Checkbox(Language.UseOverlayColors, ref useOverlayColors))
        {
            Plugin.Configuration.UseOverlayColors = useOverlayColors;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.UseOverlayColors_HelpMarker);

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.MaxItems);
        ImGuiComponents.HelpMarker(Language.MaxItems_HelpMarker);
        var maxItemsInOverlay = Plugin.Configuration.MaxItemsInOverlay;
        if (ImGui.SliderInt("###PriceCheck_MaxItems_Slider", ref maxItemsInOverlay, 1, 30))
        {
            Plugin.Configuration.MaxItemsInOverlay = maxItemsInOverlay;
            Plugin.SaveConfig();
        }

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.HideOverlayTimer);
        ImGuiComponents.HelpMarker(Language.HideOverlayTimer_HelpMarker);
        var hideOverlayTimer = Plugin.Configuration.HideOverlayElapsed / 1000;
        if (ImGui.SliderInt("###PriceCheck_HideOverlay_Slider", ref hideOverlayTimer, 0, 300))
        {
            Plugin.Configuration.HideOverlayElapsed = hideOverlayTimer * 1000;
            Plugin.SaveConfig();
        }

        ImGui.Spacing();
        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.FiltersHeading);
        ImGui.Spacing();
        var showSuccessInOverlay = Plugin.Configuration.ShowSuccessInOverlay;
        if (ImGui.Checkbox(Language.ShowSuccessInOverlay, ref showSuccessInOverlay))
        {
            Plugin.Configuration.ShowSuccessInOverlay = showSuccessInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowSuccessInOverlay_HelpMarker);

        var showFailedToProcessInOverlay = Plugin.Configuration.ShowFailedToProcessInOverlay;
        if (ImGui.Checkbox(Language.ShowFailedToProcessInOverlay, ref showFailedToProcessInOverlay))
        {
            Plugin.Configuration.ShowFailedToProcessInOverlay = showFailedToProcessInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowFailedToProcessInOverlay_HelpMarker);

        var showFailedToGetDataInOverlay = Plugin.Configuration.ShowFailedToGetDataInOverlay;
        if (ImGui.Checkbox(Language.ShowFailedToGetDataInOverlay, ref showFailedToGetDataInOverlay))
        {
            Plugin.Configuration.ShowFailedToGetDataInOverlay = showFailedToGetDataInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowFailedToGetDataInOverlay_HelpMarker);

        var showNoDataAvailableInOverlay = Plugin.Configuration.ShowNoDataAvailableInOverlay;
        if (ImGui.Checkbox(Language.ShowNoDataAvailableInOverlay, ref showNoDataAvailableInOverlay))
        {
            Plugin.Configuration.ShowNoDataAvailableInOverlay = showNoDataAvailableInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowNoDataAvailableInOverlay_HelpMarker);

        var showNoRecentDataAvailableInOverlay = Plugin.Configuration.ShowNoRecentDataAvailableInOverlay;
        if (ImGui.Checkbox(Language.ShowNoRecentDataAvailableInOverlay, ref showNoRecentDataAvailableInOverlay))
        {
            Plugin.Configuration.ShowNoRecentDataAvailableInOverlay = showNoRecentDataAvailableInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowNoRecentDataAvailableInOverlay_HelpMarker);

        var showBelowVendorInOverlay = Plugin.Configuration.ShowBelowVendorInOverlay;
        if (ImGui.Checkbox(Language.ShowBelowVendorInOverlay, ref showBelowVendorInOverlay))
        {
            Plugin.Configuration.ShowBelowVendorInOverlay = showBelowVendorInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowBelowVendorInOverlay_HelpMarker);

        var showBelowMinimumInOverlay = Plugin.Configuration.ShowBelowMinimumInOverlay;
        if (ImGui.Checkbox(Language.ShowBelowMinimumInOverlay, ref showBelowMinimumInOverlay))
        {
            Plugin.Configuration.ShowBelowMinimumInOverlay = showBelowMinimumInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowBelowMinimumInOverlay_HelpMarker);

        var showUnmarketableInOverlay = Plugin.Configuration.ShowUnmarketableInOverlay;
        if (ImGui.Checkbox(Language.ShowUnmarketableInOverlay, ref showUnmarketableInOverlay))
        {
            Plugin.Configuration.ShowUnmarketableInOverlay = showUnmarketableInOverlay;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowUnmarketableInOverlay_HelpMarker);
    }
}
