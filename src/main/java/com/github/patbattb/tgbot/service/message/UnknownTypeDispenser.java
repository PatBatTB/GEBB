package com.github.patbattb.tgbot.service.message;

import com.github.patbattb.tgbot.container.MessageContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class UnknownTypeDispenser implements TypeDispenser {
    @Override
    public void accept(MessageContainer<SendMessage> messageContainer) {
        messageContainer.getMethod().setText("Unknown.");
    }
}
