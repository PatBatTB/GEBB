using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class MenuButtonHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> CallbackMenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainMenuHandle,
        [CallbackMenu.MyEvents] = MyEventsMenuHandle,
        [CallbackMenu.CreateEvent] = CreateEventMenuHandle
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MainMenuHandlerDict = new()
    {
        [CallbackButton.MyEvents] = MainMenuMyEventsButtonHandle,
        [CallbackButton.MyRegistrations] = MainMenuMyRegistrationsButtonHandle,
        [CallbackButton.AvailableEvents] = MainMenuAvailableEventsButtonHandle,
        [CallbackButton.Close] = MenuCloseButtonHandle
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MyEventsMenuHandlerDict = new()
    {
        [CallbackButton.Create] = MyEventsMenuCreateButtonHandle,
        [CallbackButton.List] = MyEventsMenuListButtonHandle,
        [CallbackButton.Back] = MyEventsMenuBackButtonHandle
    };

    //TODO
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> CreateEventMenuHandlerDict = new()
    {
        [CallbackButton.Title] = CreateEventMenuTitleButton,
        [CallbackButton.Close] = MenuCloseButtonHandle
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

    private static void CreateEventMenuHandle(UpdateContainer container)
    {
        //TODO
        CreateEventMenuHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton)!.Invoke(container);
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
            CallbackMenu.MyEvents.Title(),
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

    private static void MenuCloseButtonHandle(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);

        container.UserEntity.UserStatus = UserStatus.Active;
        container.DatabaseHandler.Update(container.UserEntity);

        using TgbotContext db = new();
        if (db.Find<EventEntity>(messageId, chatId) is { } currentEvent)
        {
            db.Remove(currentEvent);
            db.SaveChanges();
        }
    }

    private static void MainMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MainMenuUnknownButton");
        throw new NotImplementedException();
    }

    private static async void MyEventsMenuCreateButtonHandle(UpdateContainer container)
    {
        try
        {
            var chatId = container.ChatId;
            var messageId = container.Message.Id;
            var token = container.Token;

            await container.BotClient.DeleteMessage(
                chatId,
                messageId,
                token);

            var sent = await container.BotClient.SendMessage(
                chatId,
                CallbackMenu.CreateEvent.Title(),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent),
                cancellationToken: token);

            container.UserEntity.UserStatus = UserStatus.CreateEvent;
            container.DatabaseHandler.Update(container.UserEntity);

            await using TgbotContext db = new();
            EventEntity newEvent = new()
            {
                EventId = sent.Id,
                CreatorId = container.User.Id,
                CreatedAt = DateTime.Now,
                IsActive = false,
                IsCreateCompleted = false
            };
            db.Add(newEvent);
            await db.SaveChangesAsync(token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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
            CallbackMenu.Main.Title(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void MyEventsMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MyEventsMenuUnknownButton");
        throw new NotImplementedException();
    }

    private static void CreateEventMenuTitleButton(UpdateContainer container)
    {
    }
}