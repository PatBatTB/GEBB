using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Text;

public static class CreateEventStatusHandler
{
    private static readonly Dictionary<string, Func<UpdateContainer, bool>> UpdateEventFieldDict = new()
    {
        [CreateEventStatus.Title.Message()] = UpdateTitleField,
        [CreateEventStatus.DateTimeOf.Message()] = UpdateDateTimeOfField,
        [CreateEventStatus.Address.Message()] = UpdateAddressField,
        [CreateEventStatus.Cost.Message()] = UpdateCostField,
        [CreateEventStatus.ParticipantLimit.Message()] = UpdateParticipantLimitField,
        [CreateEventStatus.Description.Message()] = UpdateDescriptionField,
    };

    private static readonly IEventService EService = new DbEventService();
    private static readonly IUserService UService = new DbUserService();

    public static void Handle(UpdateContainer container)
    {
        if (container.Message.ReplyToMessage?.From?.Id != container.BotClient.BotId) return;

        container.Events.AddRange(EService.GetInCreating(container.UserDto.UserId));
        Thread.Sleep(500);
        container.BotClient.DeleteMessages(
            container.ChatId,
            [container.Message.Id, container.Message.ReplyToMessage.Id],
            container.Token);
        
        if (container.Events.Count == 1)
        {
            EventDto currentEvent = container.Events[0];
            if (UpdateEventFieldDict.GetValueOrDefault(container.Message.ReplyToMessage!.Text!, UnknownField)
                .Invoke(container))
            {
                EService.Update(currentEvent);
            }

            container.BotClient.EditMessageReplyMarkup(
                container.ChatId,
                currentEvent.MessageId,
                InlineKeyboardProvider.GetDynamicCreateEventMarkup(currentEvent),
                cancellationToken: container.Token);
            return;
        }

        container.UserDto.UserStatus = UserStatus.Active;
        UService.Update(container.UserDto);

        container.BotClient.SetMyCommands(
            BotCommandProvider.GetCommandMenu(container.UserDto.UserStatus),
            BotCommandScope.Chat(container.ChatId),
            cancellationToken: container.Token);

        if (container.Events.Count > 1)
        {
            List<int> idList = container.Events.Select(elem => elem.MessageId).ToList();
            EService.Remove(container.Events);

            container.BotClient.DeleteMessages(
                chatId: container.ChatId,
                messageIds: idList,
                cancellationToken: container.Token);

            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Возможно произошла ошибка.\n" +
                      "Вы пытаетесь создать более одного мероприятия одновременно.\n" +
                      "Режим создания был очищен, воспользуйтесь меню для создания нового мероприятия.");
        }

        if (container.Events.Count == 0)
        {
            container.BotClient.SendMessage(
                container.ChatId,
                "У вас нет мероприятий в режиме создания.\n" +
                "Воспользуйтесь командой /menu для создания нового мероприятия.",
                cancellationToken: container.Token);
        }
    }

    private static bool UpdateTitleField(UpdateContainer container)
    {
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            string message = "Название мероприятия не может быть пустым. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventTitleReplace, message);
            return false;
        }

        container.Events[0].Title = container.Message.Text;
        return true;
    }

    private static bool UpdateDateTimeOfField(UpdateContainer container)
    {
        string dateTimeString = container.Message.Text!;

        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: [container.Message.Id, container.Message.ReplyToMessage!.Id],
            cancellationToken: container.Token);

        if (!DateTimeParser.TryParse(dateTimeString, out var nDate) || nDate is not { } date)
        {
            Console.WriteLine("Incorrect date format");
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: CallbackMenu.EventDateTimeOfAgain.Text(),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventDateTimeOfAgain),
                cancellationToken: container.Token);
            return false;
        }

        if (date <= DateTime.Now)
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Ошибка. Дата события указана в прошлом. Указать еще раз?",
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventDateTimeOfAgain),
                cancellationToken: container.Token);
            return false;
        }

        container.Events[0].DateTimeOf = date;
        return true;
    }

    private static bool UpdateAddressField(UpdateContainer container)
    {
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Адрес не может быть пустым. Ввести еще раз?",
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventAddressReplace),
                cancellationToken: container.Token);
            return false;
        }

        container.Events[0].Address = container.Message.Text;
        return true;
    }

    private static bool UpdateCostField(UpdateContainer container)
    {
        if (!int.TryParse(container.Message.Text, out int cost))
        {
            string message = "Ошибка. Необходимо ввести число. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventCostReplace, message);
            return false;
        }

        if (cost < 0)
        {
            string message = "Стоимость не может быть отрицательной. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventCostReplace, message);
            return false;
        }

        container.Events[0].Cost = cost;
        return true;
    }

    private static bool UpdateParticipantLimitField(UpdateContainer container)
    {
        if (!int.TryParse(container.Message.Text, out int count))
        {
            string message = "Ошибка. Необходимо ввести число. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventParticipantLimitReplace, message);
            return false;
        }

        if (count < 1)
        {
            string message = "Необходимо пригласить как минимум одного человека. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventParticipantLimitReplace, message);
            return false;
        }

        container.Events[0].ParticipantLimit = count;
        return true;
    }

    private static bool UpdateDescriptionField(UpdateContainer container)
    {
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            string message = "Описание не может быть пустым. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventDescriptionReplace, message);
            return false;
        }

        container.Events[0].Description = container.Message.Text;
        return true;
    }

    private static void SendEnterAgainMenu(UpdateContainer container, CallbackMenu menu, string message)
    {
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: message,
            replyMarkup: InlineKeyboardProvider.GetMarkup(menu),
            cancellationToken: container.Token);
    }

    private static bool UnknownField(UpdateContainer container)
    {
        Console.WriteLine("CreateEventHandler.UnknownField()");
        return false;
    }
}