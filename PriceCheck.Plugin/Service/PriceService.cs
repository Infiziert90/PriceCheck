using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Interface.Colors;

// ReSharper disable UseCollectionExpression
namespace PriceCheck;

/// <summary>
/// Pricing service.
/// </summary>
public class PriceService
{
    private readonly Plugin Plugin;
    private readonly List<PricedItem> PricedItems = new();
    private readonly object Locker = new();

    /// <summary>
    /// Gets or sets last price check conducted in unix timestamp.
    /// </summary>
    public long LastPriceCheck { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceService"/> class.
    /// </summary>
    /// <param name="plugin">price check plugin.</param>
    public PriceService(Plugin plugin)
    {
        Plugin = plugin;
        LastPriceCheck = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Get priced items.
    /// </summary>
    /// <returns>list of priced items.</returns>
    public IEnumerable<PricedItem> GetItems()
    {
        lock (Locker)
        {
            return PricedItems.ToList();
        }
    }

    /// <summary>
    /// Clear all items.
    /// </summary>
    public void ClearItems()
    {
        lock (Locker)
        {
            PricedItems.Clear();
        }
    }

    /// <summary>
    /// Conduct price check.
    /// </summary>
    /// <param name="itemId">item id to lookup.</param>
    /// <param name="isHQ">indicator if item is hq.</param>
    public void ProcessItemAsync(uint itemId, bool isHQ)
    {
        try
        {
            if (!Plugin.ShouldPriceCheck())
                return;

            // reject if invalid itemId
            if (itemId == 0)
            {
                Plugin.ItemCancellationTokenSource = null;
                return;
            }

            // cancel if in-flight request
            if (Plugin.ItemCancellationTokenSource != null)
            {
                if (!Plugin.ItemCancellationTokenSource.IsCancellationRequested)
                    Plugin.ItemCancellationTokenSource.Cancel();

                Plugin.ItemCancellationTokenSource.Dispose();
            }

            // create new cancel token
            Plugin.ItemCancellationTokenSource = new CancellationTokenSource(Plugin.Configuration.RequestTimeout * 2);

            // run price check
            Task.Run(async () =>
            {
                await Task.Delay(Plugin.Configuration.HoverDelay * 1000, Plugin.ItemCancellationTokenSource!.Token)
                          .ConfigureAwait(false);

                await Plugin.PriceService.ProcessItem(itemId, isHQ);
            });
        }
        catch (Exception ex)
        {
            Plugin.PluginLog.Error(ex, "Failed to process item.");
            Plugin.ItemCancellationTokenSource = null;
            Plugin.HoveredItemManager.ItemId = 0;
        }
    }

    private async Task ProcessItem(uint itemId, bool isHQ)
    {
        // reject invalid item id
        if (itemId == 0)
            return;

        // create priced item
        Plugin.PluginLog.Debug($"Pricing itemId={itemId} hq={isHQ}");
        var pricedItem = new PricedItem
        {
            ItemId = itemId,
            IsHQ = isHQ,
        };

        // run price check
        await PriceCheck(pricedItem);

        // check for existing entry for this itemId
        lock (Locker)
        {
            for (var i = 0; i < PricedItems.Count; i++)
            {
                if (PricedItems[i].ItemId != pricedItem.ItemId) continue;
                PricedItems.RemoveAt(i);
                break;
            }
        }

        // determine message and colors
        SetFieldsByResult(pricedItem);

        // add to overlay
        if (Plugin.Configuration.ShowOverlay)
        {
            // remove items over max
            lock (Locker)
            {
                while (PricedItems.Count >= Plugin.Configuration.MaxItemsInOverlay)
                {
                    PricedItems.RemoveAt(PricedItems.Count - 1);
                }
            }

            // add item depending on result
            switch (pricedItem.Result)
            {
                case ItemResult.None:
                    break;
                case ItemResult.Success:
                    if (Plugin.Configuration.ShowSuccessInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.FailedToProcess:
                    if (Plugin.Configuration.ShowFailedToProcessInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.FailedToGetData:
                    if (Plugin.Configuration.ShowFailedToGetDataInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.NoDataAvailable:
                    if (Plugin.Configuration.ShowNoDataAvailableInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.NoRecentDataAvailable:
                    if (Plugin.Configuration.ShowNoRecentDataAvailableInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.BelowVendor:
                    if (Plugin.Configuration.ShowBelowVendorInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.BelowMinimum:
                    if (Plugin.Configuration.ShowBelowMinimumInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                case ItemResult.Unmarketable:
                    if (Plugin.Configuration.ShowUnmarketableInOverlay)
                        AddItemToOverlay(pricedItem);
                    break;
                default:
                    Plugin.PluginLog.Error("Unrecognized item result.");
                    break;
            }
        }

        // send chat message
        if (Plugin.Configuration.ShowInChat)
        {
            switch (pricedItem.Result)
            {
                case ItemResult.None:
                    break;
                case ItemResult.Success:
                    if (Plugin.Configuration.ShowSuccessInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.FailedToProcess:
                    if (Plugin.Configuration.ShowFailedToProcessInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.FailedToGetData:
                    if (Plugin.Configuration.ShowFailedToGetDataInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.NoDataAvailable:
                    if (Plugin.Configuration.ShowNoDataAvailableInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.NoRecentDataAvailable:
                    if (Plugin.Configuration.ShowNoRecentDataAvailableInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.BelowVendor:
                    if (Plugin.Configuration.ShowBelowVendorInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.BelowMinimum:
                    if (Plugin.Configuration.ShowBelowMinimumInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                case ItemResult.Unmarketable:
                    if (Plugin.Configuration.ShowUnmarketableInChat)
                        Plugin.PrintItemMessage(pricedItem);
                    break;
                default:
                    Plugin.PluginLog.Error("Unrecognized item result.");
                    break;
            }
        }

        // send toast
        if (Plugin.Configuration.ShowToast)
            Plugin.SendToast(pricedItem);
    }

    private void AddItemToOverlay(PricedItem pricedItem)
    {
        Plugin.MainWindow.IsOpen = true;
        lock (Locker)
        {
            PricedItems.Insert(0, pricedItem);
        }
    }

    private void SetFieldsByResult(PricedItem pricedItem)
    {
        switch (pricedItem.Result)
        {
            case ItemResult.Success:
                pricedItem.Message = Plugin.Configuration.ShowPrices
                                         ? pricedItem.MarketPrice.ToString("N0", CultureInfo.InvariantCulture)
                                         : Language.SellOnMarketboard;
                pricedItem.OverlayColor = ImGuiColors.HealerGreen;
                pricedItem.ChatColor = 45;
                break;
            case ItemResult.None:
                pricedItem.Message = Language.FailedToGetData;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
            case ItemResult.FailedToProcess:
                pricedItem.Message = Language.FailedToProcess;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
            case ItemResult.FailedToGetData:
                pricedItem.Message = Language.FailedToGetData;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
            case ItemResult.NoDataAvailable:
                pricedItem.Message = Language.NoDataAvailable;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
            case ItemResult.NoRecentDataAvailable:
                pricedItem.Message = Language.NoRecentDataAvailable;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
            case ItemResult.BelowVendor:
                pricedItem.Message = Language.BelowVendor;
                pricedItem.OverlayColor = ImGuiColors.DalamudYellow;
                pricedItem.ChatColor = 25;
                break;
            case ItemResult.BelowMinimum:
                pricedItem.Message = Language.BelowMinimum;
                pricedItem.OverlayColor = ImGuiColors.DalamudYellow;
                pricedItem.ChatColor = 25;
                break;
            case ItemResult.Unmarketable:
                pricedItem.Message = Language.Unmarketable;
                pricedItem.OverlayColor = ImGuiColors.DalamudYellow;
                pricedItem.ChatColor = 25;
                break;
            default:
                pricedItem.Message = Language.FailedToProcess;
                pricedItem.OverlayColor = ImGuiColors.DPSRed;
                pricedItem.ChatColor = 17;
                break;
        }

        Plugin.PluginLog.Debug($"Message={pricedItem.Message}");
    }

    private async Task PriceCheck(PricedItem pricedItem)
    {
        // record current time for window visibility
        LastPriceCheck = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Plugin.PluginLog.Debug($"LastPriceCheck={LastPriceCheck}");

        if (!Sheets.ItemSheet.TryGetRow(pricedItem.ItemId, out var item))
        {
            pricedItem.Result = ItemResult.FailedToProcess;
            Plugin.PluginLog.Error($"Failed to retrieve game data for itemId {pricedItem.ItemId}.");
            return;
        }

        // set fields from game data
        pricedItem.ItemName = pricedItem.IsHQ ? $"{item.Name.ExtractText()} {SeIconChar.HighQuality.ToIconChar()}" : item.Name.ExtractText();
        Plugin.PluginLog.Debug($"ItemName={pricedItem.ItemName}");
        pricedItem.IsMarketable = item.ItemSearchCategory.RowId != 0;
        Plugin.PluginLog.Debug($"IsMarketable={pricedItem.IsMarketable}");
        pricedItem.VendorPrice = item.PriceLow;
        Plugin.PluginLog.Debug($"VendorPrice={pricedItem.VendorPrice}");

        // check if marketable
        if (!pricedItem.IsMarketable)
        {
            pricedItem.Result = ItemResult.Unmarketable;
            return;
        }

        // set worldId
        var worldId = 0u;
        await Plugin.Framework.RunOnTick(() => worldId = Plugin.ClientState.LocalPlayer?.HomeWorld.RowId ?? 0).ConfigureAwait(true);

        Plugin.PluginLog.Debug($"worldId={worldId}");
        if (worldId == 0)
        {
            pricedItem.Result = ItemResult.FailedToProcess;
            return;
        }

        // lookup market data
        MarketBoardData? marketBoardData;
        try
        {
            marketBoardData = Plugin.UniversalisClient.GetMarketBoard(worldId, pricedItem.ItemId);
        }
        catch (Exception ex)
        {
            Plugin.PluginLog.Error(ex, "Caught exception trying to get marketboard data.");
            marketBoardData = null;
        }

        // validate marketboard response
        if (marketBoardData == null)
        {
            pricedItem.Result = ItemResult.FailedToGetData;
            Plugin.PluginLog.Error("Failed to get marketboard data.");
            return;
        }

        // validate marketboard data
        if (marketBoardData.AveragePriceNQ == null || marketBoardData.LastCheckTime == 0)
        {
            pricedItem.Result = ItemResult.NoDataAvailable;
            return;
        }

        // set market price
        double? marketPrice = null;
        if (Plugin.Configuration.PriceMode == PriceMode.AveragePrice.Index)
            marketPrice = pricedItem.IsHQ ? marketBoardData.AveragePriceHQ : marketBoardData.AveragePriceNQ;
        else if (Plugin.Configuration.PriceMode == PriceMode.CurrentAveragePrice.Index)
            marketPrice = pricedItem.IsHQ ? marketBoardData.CurrentAveragePriceHQ : marketBoardData.CurrentAveragePriceNQ;
        else if (Plugin.Configuration.PriceMode == PriceMode.MinimumPrice.Index)
            marketPrice = pricedItem.IsHQ ? marketBoardData.MinimumPriceHQ : marketBoardData.MinimumPriceNQ;
        else if (Plugin.Configuration.PriceMode == PriceMode.MaximumPrice.Index)
            marketPrice = pricedItem.IsHQ ? marketBoardData.MaximumPriceHQ : marketBoardData.MaximumPriceNQ;
        else if (Plugin.Configuration.PriceMode == PriceMode.CurrentMinimumPrice.Index)
            marketPrice = marketBoardData.CurrentMinimumPrice;

        if (marketPrice is null or 0)
        {
            pricedItem.Result = ItemResult.NoDataAvailable;
            return;
        }

        marketPrice = Math.Round((double)marketPrice);
        pricedItem.MarketPrice = (uint)marketPrice;
        Plugin.PluginLog.Debug($"marketPrice={pricedItem.MarketPrice}");

        // compare with date threshold
        var diffInSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - pricedItem.LastUpdated;
        var diffInDays = diffInSeconds / 86400000;
        Plugin.PluginLog.Debug($"Max Days Check: diffDays={diffInDays} >= maxUpload={Plugin.Configuration.MaxUploadDays}");
        if (diffInDays >= Plugin.Configuration.MaxUploadDays)
        {
            pricedItem.Result = ItemResult.NoRecentDataAvailable;
            return;
        }

        // compare with vendor price
        Plugin.PluginLog.Debug($"Vendor Check: vendorPrice={pricedItem.VendorPrice} >= marketPrice={pricedItem.MarketPrice}");
        if (pricedItem.VendorPrice >= pricedItem.MarketPrice)
        {
            pricedItem.Result = ItemResult.BelowVendor;
            return;
        }

        // compare with price threshold
        Plugin.PluginLog.Debug($"Min Check: marketPrice={pricedItem.MarketPrice} < minPrice={Plugin.Configuration.MinPrice}");
        if (pricedItem.MarketPrice < Plugin.Configuration.MinPrice)
        {
            pricedItem.Result = ItemResult.BelowMinimum;
            return;
        }

        // made it - set as success
        pricedItem.Result = ItemResult.Success;
    }
}
