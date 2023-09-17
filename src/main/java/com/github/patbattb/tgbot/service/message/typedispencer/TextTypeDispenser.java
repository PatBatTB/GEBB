package com.github.patbattb.tgbot.service.message.typedispencer;

import com.github.patbattb.tgbot.container.MethodContainer;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class TextTypeDispenser implements TypeDispenser {
    @Override
    public void accept(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "Пожалуйста, воспользуйтесь меню для выбора необходимой команды.";
        methodContainer.getMethods().add(new SendMessage(chatId, message));
    }
}
