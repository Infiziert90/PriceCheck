using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Toast()
    {
        using var tabItem = ImRaii.TabItem(Language.Toast);
        if (!tabItem.Success)
            return;

        var showToast = Plugin.Configuration.ShowToast;
        if (ImGui.Checkbox(Language.ShowToast, ref showToast))
        {
            Plugin.Configuration.ShowToast = showToast;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowToast_HelpMarker);
    }
}
