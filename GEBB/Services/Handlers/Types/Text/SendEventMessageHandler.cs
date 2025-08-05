using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Text;

public class SendEventMessageHandler
{
    private readonly IUserService _userService = App.ServiceFactory.GetUserService();
    private readonly IEventMessageService _eventMessageService = App.ServiceFactory.GetEventMessageService();
    
    public void Handle(UpdateContainer container)
    {
        if (container.Message.ReplyToMessage is null ||
            container.Message.ReplyToMessage.Text != "Введите сообщение для отправки:")
        {
            HandleError(container);
        }
        else
        {
            SendMessages(container);
        }
        List<int> messageIds = [container.Message.Id];
        if (container.Message.ReplyToMessage is not null)
        {
            messageIds.Add(container.Message.ReplyToMessage.Id);
        }
        DataService.UpdateUserStatus(container, UserStatus.Active, _userService);
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            container.ChatId,
            messageIds);
    }

    private void SendMessages(UpdateContainer container)
    {
        AppEventMessage eventMessage = _eventMessageService.Get(container.ChatId);
        List<long> userIds = eventMessage.Event.RegisteredUsers.Select(e => e.UserId).ToList();
        userIds.Add(eventMessage.Event.Creator.UserId);
        userIds.Remove(eventMessage.User.UserId);
        foreach (long id in userIds)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: id,
                text: $"Мероприятие: {eventMessage.Event.Title}\n" +
                      $"Сообщение от: @{eventMessage.User.Username}\n\n" +
                      $"{container.Message.Text}");
        }
        string messageText =
            userIds.Count > 0 ? "Сообщение отправлено" : "Для данного мероприятия не найдено получателей";
        DataService.UpdateUserStatus(container, UserStatus.Active, _userService);
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: messageText,
            cancellationToken: container.Token);

    }

    private void HandleError(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text:
            """Произошла ошибка. Нажмите заново кнопку "Сообщение участникам" в карточке мероприятия и напишите сообщение в ответе за запрос.""",
            cancellationToken: container.Token
        );
    }
}