using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types;

namespace Com.GitHub.PatBatTB.GEBB.Extensions;

public static class MessageExtension
{
    public static MessageType TextType(this Message message)
    {
        if (message.Text is not { } text) return MessageType.Unknown;
        if (text == string.Empty) return MessageType.Unknown;
        if (text.TrimStart().StartsWith("/") && text.Trim().Split().Length == 1) return MessageType.Command;
        return MessageType.Text;
    }
}