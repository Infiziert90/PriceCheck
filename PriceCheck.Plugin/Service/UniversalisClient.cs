using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PriceCheck;

/// <summary>
/// Universalis client.
/// </summary>
public class UniversalisClient
{
    private const string Endpoint = "https://universalis.app/api/";
    private readonly HttpClient HttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalisClient"/> class.
    /// </summary>
    /// <param name="plugin">price check plugin.</param>
    public UniversalisClient(Plugin plugin)
    {
        HttpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(plugin.Configuration.RequestTimeout), };
    }

    /// <summary>
    /// Get market board data.
    /// </summary>
    /// <param name="worldId">world id.</param>
    /// <param name="itemId">item id.</param>
    /// <returns>market board data.</returns>
    public MarketBoardData? GetMarketBoard(uint worldId, ulong itemId)
    {
        return GetMarketBoardData(worldId, itemId);
    }

    /// <summary>
    /// Dispose client.
    /// </summary>
    public void Dispose()
    {
        HttpClient.Dispose();
    }

    private MarketBoardData? GetMarketBoardData(uint worldId, ulong itemId)
    {
        HttpResponseMessage result;
        try
        {
            result = GetMarketBoardDataAsync(worldId, itemId).Result;
        }
        catch (Exception ex)
        {
            Plugin.PluginLog.Error(ex, $"Failed to retrieve data from Universalis for itemId {itemId} / worldId {worldId}.");
            return null;
        }

        Plugin.PluginLog.Debug($"universalisResponse={result}");

        if (result.StatusCode != HttpStatusCode.OK)
        {
            Plugin.PluginLog.Error($"Failed to retrieve data from Universalis for itemId {itemId} / worldId {worldId} with HttpStatusCode {result.StatusCode}.");
            return null;
        }

        var json = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
        Plugin.PluginLog.Debug($"universalisResponseBody={json}");
        if (json == null)
        {
            Plugin.PluginLog.Error($"Failed to deserialize Universalis response for itemId {itemId} / worldId {worldId}.");
            return null;
        }

        try
        {
            var marketBoardData = new MarketBoardData
            {
                LastCheckTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                LastUploadTime = json.lastUploadTime?.Value,
                AveragePriceNQ = json.averagePriceNQ?.Value,
                AveragePriceHQ = json.averagePriceHQ?.Value,
                CurrentAveragePriceNQ = json.currentAveragePriceNQ?.Value,
                CurrentAveragePriceHQ = json.currentAveragePriceHQ?.Value,
                MinimumPriceNQ = json.minPriceNQ?.Value,
                MinimumPriceHQ = json.minPriceHQ?.Value,
                MaximumPriceNQ = json.maxPriceNQ?.Value,
                MaximumPriceHQ = json.maxPriceHQ?.Value,
                CurrentMinimumPrice = json.listings[0]?.pricePerUnit?.Value,
            };
            Plugin.PluginLog.Debug($"marketBoardData={JsonConvert.SerializeObject(marketBoardData)}");
            return marketBoardData;
        }
        catch (Exception ex)
        {
            Plugin.PluginLog.Error(ex, $"Failed to parse marketBoard data for itemId {itemId} / worldId {worldId}.");
            return null;
        }
    }

    private async Task<HttpResponseMessage> GetMarketBoardDataAsync(uint? worldId, ulong itemId)
    {
        var request = $"{Endpoint}{worldId}/{itemId}";
        Plugin.PluginLog.Debug($"universalisRequest={request}");
        return await HttpClient.GetAsync(new Uri(request));
    }
}
