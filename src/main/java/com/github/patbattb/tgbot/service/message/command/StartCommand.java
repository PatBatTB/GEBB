package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.component.DefaultUserMenu;
import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.model.UserRepository;
import lombok.Value;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.commands.SetMyCommands;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;
import org.telegram.telegrambots.meta.api.objects.commands.scope.BotCommandScopeChat;

@Service
@Value
public class StartCommand implements Command {

    UserRepository userRepository;
    DefaultUserMenu defaultUserMenu;

    @Override
    public void execute(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "Добро пожаловать! Вам теперь доступно меню пользователя.";
        var user = methodContainer.getUser();
        if (!user.isActive()) {
            user = user.toBuilder().active(true).build();
            userRepository.save(user);
        }
        methodContainer.getMethods().add(new SendMessage(chatId, message));
        methodContainer.getMethods().add(
                new SetMyCommands(
                        defaultUserMenu.getListOfCommands(), new BotCommandScopeChat(chatId), null));
    }
}
