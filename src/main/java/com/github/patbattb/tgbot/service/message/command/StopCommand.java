package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MessageContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class StopCommand implements Command {
    @Override
    public void execute(MessageContainer<SendMessage> messageContainer) {
        messageContainer.getMethod().setText("Bye! You run stop command");
    }
}