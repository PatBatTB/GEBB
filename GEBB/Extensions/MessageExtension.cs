using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types;

namespace Com.GitHub.PatBatTB.GEBB.Extensions;

public static class MessageExtension
{
    public static ContentMessageType TextType(this Message message)
    {
        if (message.Text is not { } text) return ContentMessageType.Unknown;
        if (text == string.Empty) return ContentMessageType.Unknown;
        if (text.TrimStart().StartsWith("/") && text.Trim().Split().Length == 1) return ContentMessageType.Command;
        return ContentMessageType.Text;
    }
}