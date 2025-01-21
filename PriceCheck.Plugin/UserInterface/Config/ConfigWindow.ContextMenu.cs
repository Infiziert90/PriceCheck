using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void ContextMenu()
    {
        using var tabItem = ImRaii.TabItem(Language.ContextMenu);
        if (!tabItem.Success)
            return;

        var showContextMenu = Plugin.Configuration.ShowContextMenu;
        if (ImGui.Checkbox(Language.ShowContextMenu, ref showContextMenu))
        {
            Plugin.Configuration.ShowContextMenu = showContextMenu;
            Plugin.SaveConfig();
        }
    }
}
