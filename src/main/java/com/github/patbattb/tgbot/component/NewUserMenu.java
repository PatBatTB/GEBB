package com.github.patbattb.tgbot.component;

import com.github.patbattb.tgbot.service.message.command.CommandEnum;
import lombok.Value;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.commands.BotCommand;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@Component
@Value
public class NewUserMenu {

    List<BotCommand> listOfCommands;

    public NewUserMenu() {
        listOfCommands = new ArrayList<>();
        Arrays.stream(CommandEnum.values())
                .filter(elem -> elem.getScopes().contains("new"))
                .forEach(elem -> listOfCommands.add(new BotCommand(elem.getName(), elem.getDescription())));

    }

}
