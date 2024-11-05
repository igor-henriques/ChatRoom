namespace ChatRoom.Domain.Shared;

public static class StockHelper
{
    private const string STOCK_COMMAND = "/stock=";
    public static bool IsStockCommand(string chatMessage)
    {
        return chatMessage.StartsWith(STOCK_COMMAND) && chatMessage.Replace(STOCK_COMMAND, string.Empty).Trim().Length > 0;
    }

    public static string GetStockName(this string? chatMessage)
    {
        if (string.IsNullOrEmpty(chatMessage))
        {
            return string.Empty;
        }

        return chatMessage.Replace(STOCK_COMMAND, string.Empty).Trim();
    }
}
