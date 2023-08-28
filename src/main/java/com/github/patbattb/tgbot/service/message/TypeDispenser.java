package com.github.patbattb.tgbot.service.message;

import com.github.patbattb.tgbot.container.MessageContainer;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@FunctionalInterface
public interface TypeDispenser {
    void accept(MessageContainer<SendMessage> messageContainer);
}
