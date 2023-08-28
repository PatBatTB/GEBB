package com.github.patbattb.tgbot.component;

import com.github.patbattb.tgbot.service.message.command.CommandEnum;
import lombok.Value;
import org.springframework.beans.factory.annotation.Autowired;
import org.telegram.telegrambots.meta.api.objects.commands.BotCommand;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@Value
public class BotMenu {

    List<BotCommand> listOfCommands;

    @Autowired
    public BotMenu() {
        listOfCommands = new ArrayList<>();
        Arrays.stream(CommandEnum.values())
                .forEach(com -> listOfCommands.add(new BotCommand(com.getName(), com.getDescription())));

    }

}
