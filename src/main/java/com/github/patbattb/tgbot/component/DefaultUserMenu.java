package com.github.patbattb.tgbot.component;

import com.github.patbattb.tgbot.service.message.command.CommandEnum;
import lombok.Value;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.commands.BotCommand;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@Value
@Component
public class DefaultUserMenu {

    List<BotCommand> listOfCommands;

    public DefaultUserMenu() {
        listOfCommands = new ArrayList<>();
        Arrays.stream(CommandEnum.values())
                .filter(elem -> elem.getScopes().contains("user"))
                .forEach(elem -> listOfCommands.add(new BotCommand(elem.getName(), elem.getDescription())));

    }

}
