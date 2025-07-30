using Com.Github.PatBatTB.GEBB.DataBase.Message;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Text;

public class SendEventMessageHandler
{
    private IUserService UService = new DbUserService();
    private IEventMessageService EmService = new DbEventMessageService();
    
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
        Thread.Sleep(200);
        container.BotClient.DeleteMessages(
            container.ChatId,
            messageIds);
    }

    private void SendMessages(UpdateContainer container)
    {
        AppEventMessage eventMessage = EmService.Get(container.ChatId);
        //TODO get list of recipients, forward the message for them
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: "Сообщение отправлено",
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
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }
}