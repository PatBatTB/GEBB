using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Text;

public class BuildingEventStatusHandler
{
    private readonly Dictionary<string, Func<UpdateContainer, bool>> _updateEventFieldDict;
    private readonly IEventService _eService;
    private readonly IUserService _uService;
    private readonly ILog _log;

    public BuildingEventStatusHandler()
    {
        _updateEventFieldDict = new Dictionary<string, Func<UpdateContainer, bool>>
        {
            [BuildEventStatus.CreateTitle.Message()] = UpdateTitleField,
            [BuildEventStatus.CreateDateTimeOf.Message()] = UpdateDateTimeOfField,
            [BuildEventStatus.CreateAddress.Message()] = UpdateAddressField,
            [BuildEventStatus.CreateCost.Message()] = UpdateCostField,
            [BuildEventStatus.CreateParticipantLimit.Message()] = UpdateParticipantLimitField,
            [BuildEventStatus.CreateDescription.Message()] = UpdateDescriptionField,
            [BuildEventStatus.EditTitle.Message()] = UpdateTitleField,
            [BuildEventStatus.EditDateTimeOf.Message()] = UpdateDateTimeOfField,
            [BuildEventStatus.EditAddress.Message()] = UpdateAddressField,
            [BuildEventStatus.EditCost.Message()] = UpdateCostField,
            [BuildEventStatus.EditParticipantLimit.Message()] = UpdateParticipantLimitField,
            [BuildEventStatus.EditDescription.Message()] = UpdateDescriptionField,
        };
        
        _eService = App.ServiceFactory.GetEventService();
        _uService = App.ServiceFactory.GetUserService();
        _log = LogManager.GetLogger(typeof(BuildingEventStatusHandler));
    }

    public void Handle(UpdateContainer container)
    {
        if (container.Message.ReplyToMessage?.From?.Id != container.BotClient.BotId) return;

        EventStatus status = (container.AppUser.UserStatus) switch
        {
            UserStatus.CreatingEvent => EventStatus.Creating,
            UserStatus.EditingEvent => EventStatus.Editing,
            _ => throw new InvalidOperationException("BuildingEventStatusHandler: Invalid UserStatus")
        };
        container.Events.AddRange(_eService.GetBuildEvents(container.AppUser.UserId, status));
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            container.ChatId,
            [container.Message.Id, container.Message.ReplyToMessage.Id],
            container.Token);
        
        if (container.Events.Count == 1)
        {
            AppEvent currentAppEvent = container.Events[0];
            if (_updateEventFieldDict.GetValueOrDefault(container.Message.ReplyToMessage!.Text!, UnknownField)
                .Invoke(container))
            {
                _eService.Update(currentAppEvent);
            }
            
            if (status == EventStatus.Creating)
            {
                Thread.Sleep(200);
                container.BotClient.EditMessageReplyMarkup(
                    container.ChatId,
                    currentAppEvent.MessageId,
                    InlineKeyboardProvider.GetDynamicCreateEventMarkup(currentAppEvent, CallbackMenu.CreateEvent),
                    cancellationToken: container.Token);
            }
            return;
        }
        
        DataService.UpdateUserStatus(container, UserStatus.Active, _uService);

        if (container.Events.Count > 1)
        {
            List<int> idList = container.Events.Select(elem => elem.MessageId).ToList();
            _eService.Remove(container.Events);
            Thread.Sleep(200);
            container.BotClient.DeleteMessages(
                chatId: container.ChatId,
                messageIds: idList,
                cancellationToken: container.Token);
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Возможно произошла ошибка.\n" +
                      "Вы пытаетесь создать более одного мероприятия одновременно.\n" +
                      "Режим создания был очищен, воспользуйтесь командой /menu для создания нового мероприятия.");
        }

        if (container.Events.Count == 0)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                container.ChatId,
                "У вас нет мероприятий в режиме создания.\n" +
                "Воспользуйтесь командой /menu для создания нового мероприятия.",
                cancellationToken: container.Token);
        }
    }

    private bool UpdateTitleField(UpdateContainer container)
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

    private bool UpdateDateTimeOfField(UpdateContainer container)
    {
        string dateTimeString = container.Message.Text!;

        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            chatId: container.ChatId,
            messageIds: [container.Message.Id, container.Message.ReplyToMessage!.Id],
            cancellationToken: container.Token);

        if (!DateTimeParser.TryParse(dateTimeString, out var nDate) || nDate is not { } date)
        {
            _log.Error("Incorrect date format");
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: CallbackMenu.EventDateTimeOfAgain.Text(),
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EventDateTimeOfAgain),
                cancellationToken: container.Token);
            return false;
        }

        if (date <= DateTime.Now)
        {
            Thread.Sleep(200);
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

    private bool UpdateAddressField(UpdateContainer container)
    {
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            Thread.Sleep(200);
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

    private bool UpdateCostField(UpdateContainer container)
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

    private bool UpdateParticipantLimitField(UpdateContainer container)
    {
        if (!int.TryParse(container.Message.Text, out int count))
        {
            string message = "Ошибка. Необходимо ввести число. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventPartLimitReplace, message);
            return false;
        }

        if (count < 0)
        {
            string message = 
                "Необходимо указать количество приглашенных гостей. Либо ноль, если приглашаются все желающие. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventPartLimitReplace, message);
            return false;
        }

        container.Events[0].ParticipantLimit = count;
        return true;
    }

    private bool UpdateDescriptionField(UpdateContainer container)
    {
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            string message = "Описание не может быть пустым. Ввести еще раз?";
            SendEnterAgainMenu(container, CallbackMenu.EventDescrReplace, message);
            return false;
        }

        container.Events[0].Description = container.Message.Text;
        return true;
    }

    private void SendEnterAgainMenu(UpdateContainer container, CallbackMenu menu, string message)
    {
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: message,
            replyMarkup: InlineKeyboardProvider.GetMarkup(menu),
            cancellationToken: container.Token);
    }

    private bool UnknownField(UpdateContainer container)
    {
        _log.Error("Unknown message to fill the new event's field");
        return false;
    }
}