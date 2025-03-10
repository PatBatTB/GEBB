using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class MyEventsHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.Create] = HandleCreate,
        [CallbackButton.List] = HandleList,
        [CallbackButton.Back] = HandleBack,
    };

    public static void Handle(UpdateContainer container)
    {
        ButtonHandlerDict.GetValueOrDefault(container.CallbackData!.DataButton, HandleUnknown).Invoke(container);
    }

    private static async void HandleCreate(UpdateContainer container)
    {
        try
        {
            //TODO проверка количества эвентов в режиме создания (если больше одного - предложить удалить и начать заново).
            var chatId = container.ChatId;
            var messageId = container.Message.Id;
            var token = container.Token;

            Thread.Sleep(300);

            await container.BotClient.DeleteMessage(
                chatId,
                messageId,
                token);

            Thread.Sleep(300);

            Message sent = await container.BotClient.SendMessage(
                chatId,
                CallbackMenu.CreateEvent.Text(),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreateEvent),
                cancellationToken: token);

            container.UserEntity.UserStatus = UserStatus.CreatingEvent;
            DatabaseHandler.Update(container.UserEntity);

            Thread.Sleep(300);

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

    private static void HandleList(UpdateContainer container)
    {
        throw new NotImplementedException();
    }

    private static void HandleBack(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MyEventsMenuUnknownButton()");
    }
}