using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Domain;

public class CallbackData
{
    public string Data { get; private set; }
    public CallbackMenu DataMenu { get; private set; }
    public CallBackButton DataButton { get; private set; }

    public static CallbackData? GetInstance(CallbackQuery? callbackQuery)
    {
        if (callbackQuery?.Data is null) return null;
        string[] dataString = callbackQuery.Data.Split("_");
        return new CallbackData
        {
            Data = callbackQuery.Data,
            DataMenu = Enum.Parse<CallbackMenu>(dataString[0]),
            DataButton = Enum.Parse<CallBackButton>(dataString[1])
        };
    }

    public static string GetDataString(CallbackMenu callbackMenu, CallBackButton callBackButton)
    {
        return callbackMenu + "_" + callBackButton;
    }
}