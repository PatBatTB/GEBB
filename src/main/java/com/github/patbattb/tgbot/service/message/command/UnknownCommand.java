package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MethodContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class UnknownCommand implements Command {
    @Override
    public void execute(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "Неизвестная комманда. Воспользуйтесь меню для получения списка доступных команд.";
        methodContainer.getMethods().add(new SendMessage(chatId, message));
    }
}
