using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services;

public static class MessageSender
{
    public static void SendEnterDataRequest(UpdateContainer container, BuildEventStatus status)
    {
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: status.Message(),
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: container.Token);
    }

    public static void SendReplaceDataMenu(UpdateContainer container, CallbackMenu menu)
    {
        Thread.Sleep(200);
        container.BotClient.SendMessage(
            chatId: container.ChatId,
            text: menu.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(menu),
            cancellationToken: container.Token);
    }
}