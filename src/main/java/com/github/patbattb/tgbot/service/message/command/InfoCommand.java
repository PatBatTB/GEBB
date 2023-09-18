package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.service.keyboard.KeyboardService;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.commands.DeleteMyCommands;
import org.telegram.telegrambots.meta.api.methods.menubutton.SetChatMenuButton;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;
import org.telegram.telegrambots.meta.api.objects.commands.scope.BotCommandScopeChat;
import org.telegram.telegrambots.meta.api.objects.commands.scope.BotCommandScopeDefault;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.ReplyKeyboardMarkup;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.ReplyKeyboardRemove;

import java.util.Collections;

@Service
public class InfoCommand implements Command {
    @Override
    public void execute(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "информационная команда... пока тут пусто";
        var sendMessage = new SendMessage(chatId, message);
        sendMessage.setReplyMarkup(KeyboardService.getDefaultKeyBoard());
        methodContainer.getMethods().add(sendMessage);
    }
}
