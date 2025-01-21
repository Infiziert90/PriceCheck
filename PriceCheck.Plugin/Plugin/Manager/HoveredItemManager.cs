using System;

namespace PriceCheck;

/// <summary>
/// Manage item hover events.
/// </summary>
public class HoveredItemManager
{
    private readonly Plugin Plugin;

    /// <summary>
    /// Previous id from item hover to allow for holding keybind after hover.
    /// </summary>
    public uint ItemId;

    /// <summary>
    /// Previous item quality from item hover to allow for holding keybind after hover.
    /// </summary>
    public bool ItemQuality;

    /// <summary>
    /// Initializes a new instance of the <see cref="HoveredItemManager"/> class.
    /// </summary>
    /// <param name="plugin">plugin.</param>
    public HoveredItemManager(Plugin plugin)
    {
        Plugin = plugin;
        Plugin.GameGui.HoveredItemChanged += HoveredItemChanged;
    }

    /// <summary>
    /// Dispose hover item events.
    /// </summary>
    public void Dispose()
    {
        Plugin.GameGui.HoveredItemChanged -= HoveredItemChanged;
    }

    private void HoveredItemChanged(object? sender, ulong itemId)
    {
        try
        {
            // cancel in-flight request
            if (Plugin.ItemCancellationTokenSource != null)
            {
                if (!Plugin.ItemCancellationTokenSource.IsCancellationRequested)
                    Plugin.ItemCancellationTokenSource.Cancel();

                Plugin.ItemCancellationTokenSource.Dispose();
            }

            // stop if invalid itemId
            if (itemId == 0)
                return;

            // capture itemId/quality
            uint realItemId;
            bool itemQuality;
            if (itemId >= 1000000)
            {
                realItemId = Convert.ToUInt32(itemId - 1000000);
                itemQuality = true;
            }
            else
            {
                realItemId = Convert.ToUInt32(itemId);
                itemQuality = false;
            }

            // if keybind without pre-click
            if (Plugin.Configuration is { KeybindEnabled: true, AllowKeybindAfterHover: false })
            {
                // call immediately
                if (!Plugin.IsKeyBindPressed())
                    return;

                Plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
                return;
            }

            // if keybind post-click
            if (Plugin.Configuration is { KeybindEnabled: true, AllowKeybindAfterHover: true })
            {
                if (Plugin.IsKeyBindPressed())
                {
                    // call immediately
                    Plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
                }
                else
                {
                    // save for next keybind press
                    ItemId = realItemId;
                    ItemQuality = itemQuality;
                }

                return;
            }

            // if no keybind
            if (!Plugin.Configuration.KeybindEnabled)
                Plugin.PriceService.ProcessItemAsync(realItemId, itemQuality);
        }
        catch (Exception ex)
        {
            Plugin.PluginLog.Error(ex, "Failed to price check.");
            ItemId = 0;
            Plugin.ItemCancellationTokenSource = null;
        }
    }
}
