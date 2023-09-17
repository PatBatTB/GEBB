package com.github.patbattb.tgbot.service;

import com.github.patbattb.tgbot.service.message.MessageType;
import lombok.experimental.UtilityClass;
import org.telegram.telegrambots.meta.api.objects.Update;

@UtilityClass
public class MessageParser {

    public MessageType getType(Update update) {
        var textMessage = update.getMessage().getText();
        if (textMessage == null) return MessageType.UNKNOWN;
        if (textMessage.matches("^/\\S+\\s*$")) return MessageType.COMMAND;
        if (!textMessage.isBlank()) return MessageType.TEXT;
        return MessageType.UNKNOWN;
    }
}
