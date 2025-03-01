using Com.Github.PatBatTB.GEBB.DataBase;
using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers;

public static class CreateEventHandler
{
    private static readonly Dictionary<string, Func<UpdateContainer, bool>> UpdateEventFieldDict = new()
    {
        [CreateEventStatus.Title.Message()] = UpdateTitleField
    };

    public static void Handle(UpdateContainer container)
    {
        //Проверить, что пользователь отвечает на сообщение бота.
        if (container.Message.ReplyToMessage?.From?.Id != container.BotClient.BotId) return;

        //Получить список EventEntity
        using (TgBotDBContext db = new())
        {
            container.EventEntity.AddRange(db.Events.FromSqlInterpolated(
                $@"
                    SELECT * 
                    FROM Events 
                    WHERE CreatorId = {container.User.Id} 
                    AND IsCreateCompleted == 0
                ").ToList());
        }

        //проверить, что у пользователя создается только одно мероприятие
        if (container.EventEntity.Count > 1)
        {
            using (TgBotDBContext db = new())
            {
                db.Events.FromSqlInterpolated(
                    $@"
                        DELETE FROM Events
                        WHERE CreatorId = {container.User.Id} 
                        AND IsCreateCompleted == 0
                    ");
            }

            container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: "Возможно произошла ошибка.\n" +
                      "Вы пытаетесь создать более одного мероприятия одновременно.\n" +
                      "Режим создания был очищен, воспользуйтесь меню для создания нового мероприятия.");
            return;
        }

        if (container.EventEntity.Count == 0)
        {
            container.BotClient.SendMessage(
                container.ChatId,
                "У вас нет мероприятий в режиме создания.\n" +
                "Воспользуйтесь командой /menu для создания нового мероприятия.",
                cancellationToken: container.Token);
            return;
        }

        EventEntity currentEvent = container.EventEntity[0];
        using (TgBotDBContext db = new())
        {
            //Распарсить текст исходного сообщения, обновить поле
            if (!UpdateEventFieldDict.GetValueOrDefault(container.Message.ReplyToMessage!.Text!, UnknownField)
                    .Invoke(container))
                throw new InvalidOperationException();

            //Сохранить EventEntity
            db.Update(currentEvent);
            db.SaveChanges();
        }

        Thread.Sleep(500);

        //Удалить 2 сообщения: Вопрос и ответ пользователя.
        container.BotClient.DeleteMessages(
            container.ChatId,
            [container.Message.Id, container.Message.ReplyToMessage.Id],
            container.Token);

        //заменить кнопки в меню на кнопки с галочками.
        container.BotClient.EditMessageReplyMarkup(
            container.ChatId,
            currentEvent.EventId,
            InlineKeyboardProvider.GetDynamicCreateEventMarkup(currentEvent),
            cancellationToken: container.Token);
    }

    private static bool UpdateTitleField(UpdateContainer container)
    {
        //проверка, что сообщение не пустое
        if (string.IsNullOrEmpty(container.Message.Text))
        {
            container.BotClient.SendMessage(
                container.ChatId,
                "Название мероприятия не может быть пустым",
                cancellationToken: container.Token);
            return false;
        }

        container.EventEntity[0].Title = container.Message.Text;
        return true;
    }

    private static bool UnknownField(UpdateContainer container)
    {
        Console.WriteLine("CreateEventHandler.UnknownField()");
        return false;
    }
}