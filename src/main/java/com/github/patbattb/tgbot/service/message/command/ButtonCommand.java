package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MessageContainer;
import com.github.patbattb.tgbot.service.keyboard.KeyboardService;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Service
public class ButtonCommand implements Command {
    @Override
    public void execute(MessageContainer<SendMessage> messageContainer) {
        String mask = "BUTTON_1";
        messageContainer.getMethod().setReplyMarkup(KeyboardService.getYesNoKeyboard(mask));
        messageContainer.getMethod().setText("Получилось?");
    }
}
