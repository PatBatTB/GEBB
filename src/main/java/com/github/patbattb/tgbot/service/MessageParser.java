package com.github.patbattb.tgbot.service;

import com.github.patbattb.tgbot.service.message.MessageType;
import com.github.patbattb.tgbot.container.MessageContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class MessageParser {

    public MessageType getType(MessageContainer<SendMessage> container) {
        String textMessage = container.getUpdate().getMessage().getText();
        if (textMessage == null) return MessageType.UNKNOWN;
        if (textMessage.startsWith("/")) return MessageType.COMMAND;
        if (!textMessage.isBlank()) return MessageType.TEXT;
        return MessageType.UNKNOWN;
    }
}
