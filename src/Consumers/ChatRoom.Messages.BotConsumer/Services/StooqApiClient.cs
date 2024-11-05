using System.Globalization;

namespace ChatRoom.Messages.BotConsumer.Services;

internal sealed class StooqApiClient(HttpClient httpClient) : IStooqApiClient
{
    public async Task<decimal> GetStockQuoteAsync(string stockCode)
    {
        var response = await httpClient.GetAsync($"/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var lines = content.Split('\n');
        var values = lines[1].Split(',');

        return decimal.Parse(values[6], CultureInfo.InvariantCulture);
    }
}
