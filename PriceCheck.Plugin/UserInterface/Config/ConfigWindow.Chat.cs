using System;
using Dalamud.Game.Text;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private void Chat()
    {
        using var tabItem = ImRaii.TabItem(Language.Chat);
        if (!tabItem.Success)
            return;

        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.DisplayHeading);
        ImGui.Spacing();

        var showInChat = Plugin.Configuration.ShowInChat;
        if (ImGui.Checkbox(Language.ShowInChat, ref showInChat))
        {
            Plugin.Configuration.ShowInChat = showInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowInChat_HelpMarker);

        ImGui.TextUnformatted(Language.ChatChannel);
        ImGuiComponents.HelpMarker(Language.ChatChannel_HelpMarker);
        var chatChannel = Plugin.Configuration.ChatChannel;
        ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 3);
        using (var combo = ImRaii.Combo("###PriceCheck_ChatChannel_Combo", chatChannel.ToString()))
        {
            if (combo.Success)
            {
                foreach (var type in Enum.GetValues<XivChatType>())
                {
                    if (ImGui.Selectable(type.ToString(), type == chatChannel))
                    {
                        Plugin.Configuration.ChatChannel = type;
                        Plugin.SaveConfig();
                    }
                }
            }
        }

        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.StyleHeading);
        ImGui.Spacing();

        var useChatColors = Plugin.Configuration.UseChatColors;
        if (ImGui.Checkbox(Language.UseChatColors, ref useChatColors))
        {
            Plugin.Configuration.UseChatColors = useChatColors;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.UseChatColors_HelpMarker);

        var useItemLinks = Plugin.Configuration.UseItemLinks;
        if (ImGui.Checkbox(Language.UseItemLinks, ref useItemLinks))
        {
            Plugin.Configuration.UseItemLinks = useItemLinks;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.UseItemLinks_HelpMarker);

        ImGui.Spacing();
        ImGui.TextColored(ImGuiColors.DalamudViolet, Language.FiltersHeading);
        var showSuccessInChat = Plugin.Configuration.ShowSuccessInChat;
        if (ImGui.Checkbox(Language.ShowSuccessInChat, ref showSuccessInChat))
        {
            Plugin.Configuration.ShowSuccessInChat = showSuccessInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowSuccessInChat_HelpMarker);

        var showFailedToProcessInChat = Plugin.Configuration.ShowFailedToProcessInChat;
        if (ImGui.Checkbox(Language.ShowFailedToProcessInChat, ref showFailedToProcessInChat))
        {
            Plugin.Configuration.ShowFailedToProcessInChat = showFailedToProcessInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowFailedToProcessInChat_HelpMarker);

        var showFailedToGetDataInChat = Plugin.Configuration.ShowFailedToGetDataInChat;
        if (ImGui.Checkbox(Language.ShowFailedToGetDataInChat, ref showFailedToGetDataInChat))
        {
            Plugin.Configuration.ShowFailedToGetDataInChat = showFailedToGetDataInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowFailedToGetDataInChat_HelpMarker);

        var showNoDataAvailableInChat = Plugin.Configuration.ShowNoDataAvailableInChat;
        if (ImGui.Checkbox(Language.ShowNoDataAvailableInChat, ref showNoDataAvailableInChat))
        {
            Plugin.Configuration.ShowNoDataAvailableInChat = showNoDataAvailableInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowNoDataAvailableInChat_HelpMarker);

        var showNoRecentDataAvailableInChat = Plugin.Configuration.ShowNoRecentDataAvailableInChat;
        if (ImGui.Checkbox(Language.ShowNoRecentDataAvailableInChat, ref showNoRecentDataAvailableInChat))
        {
            Plugin.Configuration.ShowNoRecentDataAvailableInChat = showNoRecentDataAvailableInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowNoRecentDataAvailableInChat_HelpMarker);

        var showBelowVendorInChat = Plugin.Configuration.ShowBelowVendorInChat;
        if (ImGui.Checkbox(Language.ShowBelowVendorInChat, ref showBelowVendorInChat))
        {
            Plugin.Configuration.ShowBelowVendorInChat = showBelowVendorInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowBelowVendorInChat_HelpMarker);

        var showBelowMinimumInChat = Plugin.Configuration.ShowBelowMinimumInChat;
        if (ImGui.Checkbox(Language.ShowBelowMinimumInChat, ref showBelowMinimumInChat))
        {
            Plugin.Configuration.ShowBelowMinimumInChat = showBelowMinimumInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowBelowMinimumInChat_HelpMarker);

        var showUnmarketableInChat = Plugin.Configuration.ShowUnmarketableInChat;
        if (ImGui.Checkbox(Language.ShowUnmarketableInChat, ref showUnmarketableInChat))
        {
            Plugin.Configuration.ShowUnmarketableInChat = showUnmarketableInChat;
            Plugin.SaveConfig();
        }

        ImGuiComponents.HelpMarker(Language.ShowUnmarketableInChat_HelpMarker);
    }
}
