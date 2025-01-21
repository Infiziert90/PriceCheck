using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace PriceCheck.Config;

public partial class ConfigWindow
{
    private const float SeparatorPadding = 1.0f;
    private static float GetSeparatorPaddingHeight => SeparatorPadding * ImGuiHelpers.GlobalScale;

    private static void About()
    {
        using var tabItem = ImRaii.TabItem("About");
        if (!tabItem.Success)
            return;

        var buttonHeight = ImGui.GetFrameHeightWithSpacing() + ImGui.GetStyle().WindowPadding.Y + GetSeparatorPaddingHeight;
        using (var contentChild = ImRaii.Child("AboutContent", new Vector2(0, -buttonHeight)))
        {
            if (contentChild)
            {
                ImGuiHelpers.ScaledDummy(5.0f);

                ImGui.TextUnformatted("Author:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedGold, Plugin.PluginInterface.Manifest.Author);

                ImGui.TextUnformatted("Discord:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedGold, "@infi");

                ImGui.TextUnformatted("Version:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedOrange, Plugin.PluginInterface.Manifest.AssemblyVersion.ToString());
            }
        }

        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(1.0f);

        using var bottomChild = ImRaii.Child("AboutBottomBar", new Vector2(0, 0), false, 0);
        if (bottomChild)
        {
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedBlue))
            {
                if (ImGui.Button("Discord Thread"))
                    Dalamud.Utility.Util.OpenLink("https://discord.com/channels/581875019861328007/1019651078712721419");
            }

            ImGui.SameLine();

            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DPSRed))
            {
                if (ImGui.Button("Issues"))
                    Dalamud.Utility.Util.OpenLink("https://github.com/Infiziert90/PriceCheck/issues");
            }

            ImGui.SameLine();

            using (ImRaii.PushColor(ImGuiCol.Button, new Vector4(0.12549f, 0.74902f, 0.33333f, 0.6f)))
            {
                if (ImGui.Button("Ko-Fi Tip"))
                    Dalamud.Utility.Util.OpenLink("https://ko-fi.com/infiii");
            }
        }
    }
}
