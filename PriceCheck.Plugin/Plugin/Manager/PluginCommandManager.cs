using Dalamud.Game.Command;

namespace PriceCheck;

/// <summary>
/// Manage plugin commands.
/// </summary>
public class PluginCommandManager
{
    private readonly Plugin Plugin;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginCommandManager"/> class.
    /// </summary>
    /// <param name="plugin">plugin.</param>
    public PluginCommandManager(Plugin plugin)
    {
        this.Plugin = plugin;
        Plugin.CommandManager.AddHandler("/pcheck", new CommandInfo(this.TogglePriceCheck)
        {
            HelpMessage = "Show price check.",
            ShowInHelp = true,
        });
        Plugin.CommandManager.AddHandler("/pricecheck", new CommandInfo(this.TogglePriceCheck)
        {
            ShowInHelp = false,
        });
        Plugin.CommandManager.AddHandler("/pcheckconfig", new CommandInfo(this.ToggleConfig)
        {
            HelpMessage = "Show price check config.",
            ShowInHelp = true,
        });
        Plugin.CommandManager.AddHandler("/pricecheckconfig", new CommandInfo(this.ToggleConfig)
        {
            ShowInHelp = false,
        });
    }

    /// <summary>
    /// Dispose command manager.
    /// </summary>
    public static void Dispose()
    {
        Plugin.CommandManager.RemoveHandler("/pcheck");
        Plugin.CommandManager.RemoveHandler("/pricecheck");
        Plugin.CommandManager.RemoveHandler("/pcheckconfig");
        Plugin.CommandManager.RemoveHandler("/pricecheckconfig");
    }

    private void ToggleConfig(string command, string args)
    {
        Plugin.ConfigWindow.Toggle();
    }

    private void TogglePriceCheck(string command, string args)
    {
        this.Plugin.Configuration.ShowOverlay = !this.Plugin.Configuration.ShowOverlay;
        Plugin.MainWindow.Toggle();
    }
}
