using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class MenuButtonHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> CallbackMenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainMenuHandle,
        [CallbackMenu.MyEvents] = MyEventsMenuHandle
    };

    private static readonly Dictionary<CallBackButton, Action<UpdateContainer>> MainMenuHandlerDict = new()
    {
        [CallBackButton.MyEvents] = MainMenuMyEventsButtonHandle,
        [CallBackButton.MyRegistrations] = MainMenuMyRegistrationsButtonHandle,
        [CallBackButton.AvailableEvents] = MainMenuAvailableEventsButtonHandle,
        [CallBackButton.Close] = MainMenuCloseButtonHandle
    };

    private static readonly Dictionary<CallBackButton, Action<UpdateContainer>> MyEventsMenuHandlerDict = new()
    {
        [CallBackButton.Create] = MyEventsMenuCreateButtonHandle,
        [CallBackButton.List] = MyEventsMenuListButtonHandle,
        [CallBackButton.Back] = MyEventsMenuBackButtonHandle
    };

    public static void Handle(UpdateContainer container)
    {
        CallbackMenuHandlerDict.GetValueOrDefault(container.CallbackData!.DataMenu, CallbackUnknownMenu)
            .Invoke(container);
    }

    private static void MainMenuHandle(UpdateContainer container)
    {
        MainMenuHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton, MainMenuUnknownButtonHandle)
            .Invoke(container);
    }

    private static void MyEventsMenuHandle(UpdateContainer container)
    {
        MyEventsMenuHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton, MyEventsMenuUnknownButtonHandle)
            .Invoke(container);
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Console.WriteLine("CallbackMenuUnknown");
        throw new NotImplementedException();
    }

    private static void MainMenuMyEventsButtonHandle(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.MyEvents.Message(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.MyEvents),
            cancellationToken: container.Token);
    }

    private static void MainMenuMyRegistrationsButtonHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void MainMenuAvailableEventsButtonHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void MainMenuCloseButtonHandle(UpdateContainer container)
    {
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
    }

    private static void MainMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MainMenuUnknownButton");
        throw new NotImplementedException();
    }

    private static void MyEventsMenuCreateButtonHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void MyEventsMenuListButtonHandle(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void MyEventsMenuBackButtonHandle(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.Main.Message(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void MyEventsMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MyEventsMenuUnknownButton");
        throw new NotImplementedException();
    }
}