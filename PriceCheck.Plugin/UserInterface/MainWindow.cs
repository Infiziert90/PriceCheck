using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;

namespace PriceCheck;

/// <summary>
/// Config window for the Plugin.
/// </summary>
public class MainWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="plugin">PriceCheck Plugin.</param>
    public MainWindow(Plugin plugin) : base("PriceCheck")
    {
        Plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 150),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        RespectCloseHotkey = false;

        UpdateWindowTitle();
        if (Plugin.ClientState.IsLoggedIn)
            OpenOnLogin();
    }

    public void Dispose() { }

    /// <inheritdoc />
    public override void OnClose()
    {
        Plugin.Configuration.ShowOverlay = false;
        Plugin.SaveConfig();
    }

    /// <inheritdoc />
    public override bool DrawConditions()
    {
        if (Plugin.Configuration.HideOverlayElapsed != 0 && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Plugin.PriceService.LastPriceCheck > Plugin.Configuration.HideOverlayElapsed)
            if (!(Plugin.Configuration.ShowOverlayByKeybind && Plugin.IsKeyBindPressed()))
                return false;

        return true;
    }

    /// <summary>
    /// Open window on login depending on config.
    /// </summary>
    public void OpenOnLogin()
    {
        if (Plugin.Configuration is { ShowOverlay: true, ShowOverlayOnLogin: true })
            IsOpen = true;
    }

    /// <summary>
    /// Update window title.
    /// </summary>
    public void UpdateWindowTitle()
    {
        if (Plugin.Configuration is { ShowKeybindInTitleBar: true, KeybindEnabled: true })
            WindowName = Plugin.Configuration.PrimaryKey.Equals(PrimaryKey.Enum.VkNone)
                             ? Language.TitleBarWithKeybind1.Format(ModifierKey.Names[ModifierKey.EnumToIndex(Plugin.Configuration.ModifierKey)])
                             : Language.TitleBarWithKeybind2.Format(ModifierKey.Names[ModifierKey.EnumToIndex(Plugin.Configuration.ModifierKey)], PrimaryKey.Names[PrimaryKey.EnumToIndex(Plugin.Configuration.PrimaryKey)]);
        else
            WindowName = "PriceCheck";
    }

    /// <inheritdoc />
    public override void Draw()
    {
        if (!Plugin.Configuration.Enabled)
        {
            ImGui.TextUnformatted(Language.PluginDisabled);
            return;
        }

        try
        {
            var items = Plugin.PriceService.GetItems().ToList();
            if (items is { Count: > 0 })
            {
                using (ImRaii.Group())
                {
                    ImGui.Columns(2);
                    foreach (var item in items)
                    {
                        using var color = ImRaii.PushColor(ImGuiCol.Text, item.OverlayColor, Plugin.Configuration.UseOverlayColors);
                        ImGui.TextUnformatted(item.ItemName);
                        ImGui.NextColumn();
                        ImGui.TextUnformatted(item.Message);

                        ImGui.NextColumn();
                        ImGui.Separator();
                    }
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup("###PriceCheck_Overlay_Popup");
                }
            }
            else
            {
                ImGui.TextUnformatted(Language.WaitingForItems);
            }
        }
        catch
        {
            // ignored
        }

        using var popup = ImRaii.Popup("###PriceCheck_Overlay_Popup");
        if (!popup.Success)
            return;

        if (ImGui.MenuItem(Language.ClearPricedItems))
            Plugin.PriceService.ClearItems();
    }
}
