package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MessageContainer;
import lombok.Value;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

import java.util.HashMap;
import java.util.Map;

@Value
@Component
public class CommandMap {

    Map<String, Command> commandMap;
    Command defaultCommand;

    @Autowired
    public CommandMap(StartCommand startCommand, StopCommand stopCommand, InfoCommand infoCommand,
                      HelpCommand helpCommand, ButtonCommand buttonCommand, UnknownCommand unknownCommand) {
        commandMap = new HashMap<>() {{
            put(CommandEnum.START.getName(), startCommand);
            put(CommandEnum.STOP.getName(), stopCommand);
            put(CommandEnum.INFO.getName(), infoCommand);
            put(CommandEnum.HELP.getName(), helpCommand);
            put(CommandEnum.BUTTON.getName(), buttonCommand);
        }};
        defaultCommand = unknownCommand;
    }

    public void runCommand(String textCommand, MessageContainer<SendMessage> messageContainer) {
        commandMap.getOrDefault(textCommand, defaultCommand).execute(messageContainer);
    }
}
