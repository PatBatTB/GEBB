package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.component.NewUserMenu;
import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.model.UserRepository;
import lombok.Value;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.commands.SetMyCommands;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;
import org.telegram.telegrambots.meta.api.objects.commands.scope.BotCommandScopeChat;

@Service
@Value
public class StopCommand implements Command {

    UserRepository userRepository;
    NewUserMenu newUserMenu;

    @Override
    public void execute(MethodContainer methodContainer) {
        var user = methodContainer.getUser();
        var chatId = methodContainer.getUpdate().getMessage().getChatId().toString();
        var message = "Вы приостановили сервис, вы больше не будете получать уведомлений.\n" +
                "Для возобновления введите команду /start";
        if (user.isActive()) {
            user = user.toBuilder().active(false).build();
            userRepository.save(user);
        }
        methodContainer.getMethods().add(new SendMessage(chatId, message));
        methodContainer.getMethods().add(
                new SetMyCommands(
                        newUserMenu.getListOfCommands(), new BotCommandScopeChat(chatId), null));

    }
}
