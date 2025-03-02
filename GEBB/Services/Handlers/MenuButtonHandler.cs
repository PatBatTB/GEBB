using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class MenuButtonHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> CallbackMenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainMenuHandle,
        [CallbackMenu.MyEvents] = MyEventsMenuHandle,
        [CallbackMenu.CreateEvent] = CreateEventMenuHandle,
        [CallbackMenu.EventTitleReplace] = EventTitleReplaceHandle,
        [CallbackMenu.EventDateTimeOfAgain] = EventDateTimeOfReplaceHandle,
        [CallbackMenu.EventDateTimeOfReplace] = EventDateTimeOfReplaceHandle,
        [CallbackMenu.EventAddressReplace] = EventAddressReplaceHandle,
        [CallbackMenu.EventCostReplace] = EventCostReplaceHandle,
        [CallbackMenu.EventParticipantLimitReplace] = EventParticipantLimitReplaceHandle,
        [CallbackMenu.EventDescriptionReplace] = EventDescriptionReplaceHandle,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MainMenuHandlerDict = new()
    {
        [CallbackButton.MyEvents] = MainMenuMyEventsButtonHandle,
        [CallbackButton.MyRegistrations] = MainMenuMyRegistrationsButtonHandle,
        [CallbackButton.AvailableEvents] = MainMenuAvailableEventsButtonHandle,
        [CallbackButton.Close] = MenuCloseButtonHandle,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MyEventsMenuHandlerDict = new()
    {
        [CallbackButton.Create] = MyEventsMenuCreateButtonHandle,
        [CallbackButton.List] = MyEventsMenuListButtonHandle,
        [CallbackButton.Back] = MyEventsMenuBackButtonHandle,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> CreateEventMenuHandlerDict = new()
    {
        [CallbackButton.Title] = CreateEventMenuTitleButtonHandle,
        [CallbackButton.TitleDone] = CreateEventMenuTitleDoneButtonHandle,
        [CallbackButton.DateTimeOf] = CreateEventMenuDateTimeButtonHandle,
        [CallbackButton.DateTimeOfDone] = CreateEventMenuDateTimeDoneButtonHandle,
        [CallbackButton.Address] = CreateEventMenuAddressButtonHandle,
        [CallbackButton.AddressDone] = CreateEventMenuAddressDoneButtonHandle,
        [CallbackButton.Cost] = CreateEventMenuCostButtonHandle,
        [CallbackButton.CostDone] = CreateEventMenuCostDoneButtonHandle,
        [CallbackButton.ParticipantLimit] = CreateEventMenuParticipantLimitButtonHandle,
        [CallbackButton.ParticipantLimitDone] = CreateEventMenuParticipantLimitDoneButtonHandle,
        [CallbackButton.Description] = CreateEventMenuDescriptionButtonHandle,
        [CallbackButton.DescriptionDone] = CreateEventMenuDescriptionDoneButtonHandle,
        [CallbackButton.FinishCreating] = CreateEventMenuFinishCreatingButtonHandle,
        [CallbackButton.Close] = CreateEventMenuCloseButtonHandle,
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
        using (TgBotDBContext db = new())
        {
            container.EventEntities.AddRange(
                db.Events.AsEnumerable()
                    .Where(elem => elem.CreatorId == container.UserEntity.UserId &&
                                   elem.IsCreateCompleted == false));
        }

        int count = container.EventEntities.Count;
        if (count is 0 or > 1)
        {
            container.BotClient.DeleteMessage(
                container.ChatId,
                container.Message.MessageId,
                container.Token);
            container.UserEntity.UserStatus = UserStatus.Active;
            DatabaseHandler.Update(container.UserEntity);
            string message = (count == 0)
                ? "Ошибка. Не обнаружено мероприятий в режиме создания.\nПопробуте снова через команду /menu"
                : "Ошибка. Обнаружено несколько мероприятий в режиме создания.\nПопробуйте снова через команду /menu";
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: message,
                cancellationToken: container.Token);
            return;
        }

        CreateEventMenuHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton)!.Invoke(container);
    }

    private static void EventTitleReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuTitleButtonHandle(container);
        }
    }

    private static void EventDateTimeOfReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuDateTimeButtonHandle(container);
        }
    }

    private static void EventAddressReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuAddressButtonHandle(container);
        }
    }

    private static void EventCostReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuCostButtonHandle(container);
        }
    }

    private static void EventParticipantLimitReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuParticipantLimitButtonHandle(container);
        }
    }

    private static void EventDescriptionReplaceHandle(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);
        if (container.CallbackData!.DataButton == CallbackButton.Yes)
        {
            CreateEventMenuDescriptionButtonHandle(container);
        }
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CallbackMenuUnknown()");
    }

    private static void MainMenuMyEventsButtonHandle(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.MyEvents.Text(),
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
        DatabaseHandler.Update(container.UserEntity);

        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);
    }

    private static void MainMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MainMenuUnknownButtonHandle()");
    }

    private static async void MyEventsMenuCreateButtonHandle(UpdateContainer container)
    {
        try
        {
            //TODO проверка количества эвентов в режиме создания (если больше одного - предложить удалить и начать заново).
            var chatId = container.ChatId;
            var messageId = container.Message.Id;
            var token = container.Token;

            await container.BotClient.DeleteMessage(
                chatId,
                messageId,
                token);

            Message sent = await container.BotClient.SendMessage(
                chatId,
                CallbackMenu.CreateEvent.Text(),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent),
                cancellationToken: token);

            container.UserEntity.UserStatus = UserStatus.CreatingEvent;
            DatabaseHandler.Update(container.UserEntity);

            await container.BotClient.SetMyCommands(
                BotCommandProvider.GetCommandMenu(container.UserEntity.UserStatus),
                BotCommandScope.Chat(container.ChatId),
                cancellationToken: container.Token);

            await using TgBotDBContext db = new();
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
            CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void MyEventsMenuUnknownButtonHandle(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MyEventsMenuUnknownButton()");
    }

    private static void CreateEventMenuTitleButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.Title);
    }

    private static void CreateEventMenuTitleDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventTitleReplace);
    }

    private static void CreateEventMenuDateTimeButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.DateTimeOf);
    }

    private static void CreateEventMenuDateTimeDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventDateTimeOfReplace);
    }

    private static void CreateEventMenuAddressButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.Address);
    }

    private static void CreateEventMenuAddressDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventAddressReplace);
    }

    private static void CreateEventMenuCostButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.Cost);
    }

    private static void CreateEventMenuCostDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventCostReplace);
    }

    private static void CreateEventMenuParticipantLimitButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.ParticipantLimit);
    }

    private static void CreateEventMenuParticipantLimitDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventParticipantLimitReplace);
    }

    private static void CreateEventMenuDescriptionButtonHandle(UpdateContainer container)
    {
        SendEnterDataRequest(container, CreateEventStatus.Description);
    }

    private static void CreateEventMenuDescriptionDoneButtonHandle(UpdateContainer container)
    {
        SendReplaceDataMenu(container, CallbackMenu.EventDescriptionReplace);
    }

    private static void CreateEventMenuFinishCreatingButtonHandle(UpdateContainer container)
    {
        //Проверить все ли данные введены в EventEntity?
        EventEntity entity = container.EventEntities[0];
        string message;
        if (entity.Title is null ||
            entity.DateTimeOf is null ||
            entity.Address is null ||
            entity.Cost is null ||
            entity.ParticipantLimit is null)
        {
            message = "Сначала укажите все данные для мероприятия.";
            container.BotClient.AnswerCallbackQuery(container.CallbackData!.Id, message, true,
                cancellationToken: container.Token);
            return;
        }

        //отправить уведомление, что мероприятие создано
        message = "Мероприятие создано. Рассылаю приглашения.";
        container.BotClient.AnswerCallbackQuery(container.CallbackData!.Id, message, true,
            cancellationToken: container.Token);
        //Изменить статус IsCreateComplete на true
        entity.IsCreateCompleted = true;
        DatabaseHandler.Update(entity);
        //изменить статус пользователя
        container.UserEntity.UserStatus = UserStatus.Active;
        DatabaseHandler.Update(container.UserEntity);
        //Закрыть меню
        container.BotClient.DeleteMessage(
            container.ChatId,
            container.Message.Id,
            container.Token);

        //TODO Запросить лист пользователей (все пользователи, кроме инициатора, не со статусом stop)
        //TODO Отправить приглашения всем пользователям из списка.
    }

    private static void SendEnterDataRequest(UpdateContainer container, CreateEventStatus status)
    {
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: status.Message(),
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: container.Token);
    }

    private static void SendReplaceDataMenu(UpdateContainer container, CallbackMenu menu)
    {
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: menu.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(menu),
            cancellationToken: container.Token);
    }

    private static void CreateEventMenuCloseButtonHandle(UpdateContainer container)
    {
        MenuCloseButtonHandle(container);
        using TgBotDBContext db = new();
        if (db.Find<EventEntity>(container.Message.MessageId, container.ChatId) is { } currentEvent)
        {
            db.Remove(currentEvent);
            db.SaveChanges();
        }
    }

    private static void CreateEventMenuUnknownButton(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CreateEventMenuUnknownButton()");
    }
}