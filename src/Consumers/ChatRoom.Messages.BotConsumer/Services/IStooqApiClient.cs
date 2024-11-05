
namespace ChatRoom.Messages.BotConsumer.Services;

internal interface IStooqApiClient
{
    Task<decimal> GetStockQuoteAsync(string stockCode);
}