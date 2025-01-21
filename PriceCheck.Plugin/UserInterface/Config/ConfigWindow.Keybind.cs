using System.Linq;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Keybind()
    {
        using var tabItem = ImRaii.TabItem(Language.Keybind);
        if (!tabItem.Success)
            return;

        var keybindEnabled = Plugin.Configuration.KeybindEnabled;
        if (ImGui.Checkbox(Language.KeybindEnabled, ref keybindEnabled))
        {
            Plugin.Configuration.KeybindEnabled = keybindEnabled;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.KeybindEnabled_HelpMarker);

        var showKeybindInTitleBar = Plugin.Configuration.ShowKeybindInTitleBar;
        if (ImGui.Checkbox(Language.ShowKeybindInTitleBar, ref showKeybindInTitleBar))
        {
            Plugin.Configuration.ShowKeybindInTitleBar = showKeybindInTitleBar;
            Plugin.SaveConfig();
            Plugin.MainWindow.UpdateWindowTitle();
        }

        ImGuiComponents.HelpMarker(Language.ShowKeybindInTitleBar_HelpMarker);

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.ModifierKeybind);
        ImGuiComponents.HelpMarker(Language.ModifierKeybind_HelpMarker);
        var modifierKey = ModifierKey.EnumToIndex(Plugin.Configuration.ModifierKey);
        if (ImGui.Combo("###PriceCheck_ModifierKey_Combo", ref modifierKey, ModifierKey.Names.ToArray(), ModifierKey.Names.Length))
        {
            Plugin.Configuration.ModifierKey = ModifierKey.IndexToEnum(modifierKey);
            Plugin.SaveConfig();
            Plugin.MainWindow.UpdateWindowTitle();
        }

        ImGui.Spacing();
        ImGui.TextUnformatted(Language.PrimaryKeybind);
        ImGuiComponents.HelpMarker(Language.PrimaryKeybind_HelpMarker);
        var primaryKey = PrimaryKey.EnumToIndex(Plugin.Configuration.PrimaryKey);
        if (ImGui.Combo("###PriceCheck_PrimaryKey_Combo", ref primaryKey, PrimaryKey.Names.ToArray(), PrimaryKey.Names.Length))
        {
            Plugin.Configuration.PrimaryKey = PrimaryKey.IndexToEnum(primaryKey);
            Plugin.SaveConfig();
            Plugin.MainWindow.UpdateWindowTitle();
        }
    }
}
