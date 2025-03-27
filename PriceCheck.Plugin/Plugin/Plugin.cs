using System;
using System.Globalization;
using System.Threading;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using PriceCheck.Config;

// ReSharper disable MemberInitializerValueIgnored
namespace PriceCheck;

/// <summary>
/// PriceCheck plugin.
/// </summary>
public class Plugin : IDalamudPlugin
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IToastGui Toast { get; private set; } = null!;
    [PluginService] public static IKeyState KeyState { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static ICondition Condition { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;

    /// <summary>
    /// Cancellation token to terminate request if interrupted.
    /// </summary>
    public CancellationTokenSource? ItemCancellationTokenSource;

    /// <summary>
    /// Gets or sets command manager to handle user commands.
    /// </summary>
    public PluginCommandManager PluginCommandManager { get; set; } = null!;

    /// <summary>
    /// Gets price service.
    /// </summary>
    public PriceService PriceService { get; private set; } = null!;

    /// <summary>
    /// Gets universalis client.
    /// </summary>
    public UniversalisClient UniversalisClient { get; private set; } = null!;

    /// <summary>
    /// Gets or sets plugin configuration.
    /// </summary>
    public PriceCheckConfig Configuration { get; set; } = null!;

    /// <summary>
    /// Gets or sets hovered item manager to handle hover item events.
    /// </summary>
    public HoveredItemManager HoveredItemManager { get; set; } = null!;

    private readonly WindowSystem WindowSystem = new("PriceCheckMainWindowSystem");
    public readonly MainWindow MainWindow;
    public readonly ConfigWindow ConfigWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    public Plugin()
    {
        LanguageChanged(PluginInterface.UiLanguage);

        LoadConfig();
        HandleFreshInstall();
        PriceService = new PriceService(this);
        PluginCommandManager = new PluginCommandManager(this);
        UniversalisClient = new UniversalisClient(this);
        HoveredItemManager = new HoveredItemManager(this);
        ClientState.Login += Login;
        PluginInterface.LanguageChanged += LanguageChanged;

        MainWindow = new MainWindow(this);
        ConfigWindow = new ConfigWindow(this);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ConfigWindow);

        PluginInterface.UiBuilder.Draw += this.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
    }

    /// <summary>
    /// Dispose plugin.
    /// </summary>
    public void Dispose()
    {
        try
        {
            PluginInterface.UiBuilder.Draw -= this.Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;

            ClientState.Login -= Login;
            PluginInterface.LanguageChanged -= LanguageChanged;
            WindowSystem.RemoveAllWindows();
            MainWindow.Dispose();
            ConfigWindow.Dispose();


            PluginCommandManager.Dispose();
            HoveredItemManager.Dispose();
            ItemCancellationTokenSource?.Dispose();
            UniversalisClient.Dispose();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to dispose plugin properly.");
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sets the language to be used for loc.
    /// </summary>
    private void LanguageChanged(string langCode)
    {
        Language.Culture = new CultureInfo(langCode);
    }

    /// <summary>
    /// Gets plugin configuration.
    /// </summary>
    public static void PrintHelpMessage()
    {
        Chat.Print(Language.HelpMessage);
    }

    /// <summary>
    /// Send toast.
    /// </summary>
    /// <param name="pricedItem">priced Item.</param>
    public static void SendToast(PricedItem pricedItem)
    {
        Toast.ShowNormal($"{pricedItem.ItemName} {(char)SeIconChar.ArrowRight} {pricedItem.Message}");
    }

    /// <summary>
    /// Print item message.
    /// </summary>
    /// <param name="pricedItem">priced item.</param>
    public void PrintItemMessage(PricedItem pricedItem)
    {
        var builder = new SeStringBuilder();
        builder.AddUiForeground($"[{PluginInterface.InternalName}] ", 548);

        if (Configuration.UseChatColors)
            builder.AddUiForeground(pricedItem.ChatColor);

        if (Configuration.UseItemLinks)
            builder.AddItemLink(pricedItem.ItemId, pricedItem.IsHQ);
        else
            builder.AddText(pricedItem.ItemName);

        builder.AddText($" {SeIconChar.ArrowRight.ToIconChar()} {pricedItem.Message}");
        if (Configuration.UseChatColors)
            builder.AddUiForegroundOff();

        if (Configuration.ChatChannel == XivChatType.None)
            Chat.Print(builder.BuiltString, PluginInterface.InternalName);
        else
            Chat.Print(new XivChatEntry { Message = builder.BuiltString, Type = Configuration.ChatChannel });
    }

    /// <summary>
    /// Check if keybind is pressed.
    /// </summary>
    /// <returns>indicator if keybind is pressed.</returns>
    public bool IsKeyBindPressed()
    {
        if (!Configuration.KeybindEnabled)
            return true;

        if (Configuration.PrimaryKey == PrimaryKey.Enum.VkNone)
            return KeyState[(byte)Configuration.ModifierKey];

        return KeyState[(byte)Configuration.ModifierKey] && KeyState[(byte)Configuration.PrimaryKey];
    }

    /// <summary>
    /// Save plugin configuration.
    /// </summary>
    public void SaveConfig()
    {
        PluginInterface.SavePluginConfig((IPluginConfiguration)Configuration);
    }

    /// <summary>
    /// Check state and configuration to determine if price check should be made.
    /// </summary>
    /// <returns>indicator whether or not price check should be made.</returns>
    public bool ShouldPriceCheck()
    {
        if (Configuration.Enabled &&
            ClientState.LocalPlayer?.HomeWorld != null &&
            !(Configuration.RestrictInCombat && Condition[ConditionFlag.InCombat]) &&
            !(Configuration.RestrictInContent && Util.CheckContent(ClientState.TerritoryType)))
        {
            return true;
        }

        ItemCancellationTokenSource = null;
        return false;
    }

    private void Login()
    {
        MainWindow.OpenOnLogin();
    }

    private void HandleFreshInstall()
    {
        try
        {
            if (!Configuration.FreshInstall) return;
            Chat.Print(Language.InstallThankYou);
            PrintHelpMessage();
            Configuration.FreshInstall = false;
            Configuration.ShowToast = true;
            Configuration.RestrictInCombat = true;
            Configuration.RestrictInContent = true;
            SaveConfig();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed fresh install.");
        }
    }

    private void LoadConfig()
    {
        try
        {
            Configuration = PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
        }
        catch (Exception ex)
        {
            PluginLog.Error("Failed to load config so creating new one.", ex);
            Configuration = new PluginConfig();
            SaveConfig();
        }
    }

    private void Draw()
    {
        // only show when logged in
        if (!ClientState.IsLoggedIn)
            return;

        // run keybind post-click check to use item id set in hover manager
        if (Configuration is { KeybindEnabled: true, AllowKeybindAfterHover: true } && IsKeyBindPressed())
        {
            // call price check if item is set from previous hover
            if (HoveredItemManager.ItemId != 0)
            {
                PriceService.ProcessItemAsync(HoveredItemManager.ItemId, HoveredItemManager.ItemQuality);
                HoveredItemManager.ItemId = 0;
            }
        }

        WindowSystem.Draw();
    }

    private void OpenConfigUi()
    {
        this.ConfigWindow.Toggle();
    }
}
