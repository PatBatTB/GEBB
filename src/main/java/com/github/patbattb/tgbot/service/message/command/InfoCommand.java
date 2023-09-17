package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MethodContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class InfoCommand implements Command {
    @Override
    public void execute(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "информационная команда... пока тут пусто";
        methodContainer.getMethods().add(new SendMessage(chatId, message));
    }
}
