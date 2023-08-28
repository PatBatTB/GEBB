package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MessageContainer;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@FunctionalInterface
public interface Command {
    void execute(MessageContainer<SendMessage> messageContainer);
}
