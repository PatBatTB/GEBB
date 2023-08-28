package com.github.patbattb.tgbot.service.message;

import com.github.patbattb.tgbot.service.message.command.CommandMap;
import com.github.patbattb.tgbot.container.MessageContainer;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
@RequiredArgsConstructor
public class CommandTypeDispenser implements TypeDispenser {

    private final CommandMap commandMap;

    @Override
    public void accept(MessageContainer<SendMessage> messageContainer) {
        String textCommand = messageContainer.getUpdate().getMessage().getText();
        commandMap.runCommand(textCommand, messageContainer);
    }
}
