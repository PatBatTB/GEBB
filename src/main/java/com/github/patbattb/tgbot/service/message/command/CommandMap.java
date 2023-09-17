package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MethodContainer;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashMap;
import java.util.Map;

@Component
public class CommandMap {

    Map<String, Command> commandMap;
    Command defaultCommand;

    @Autowired
    public CommandMap(StartCommand startCommand, StopCommand stopCommand, InfoCommand infoCommand,
                      UnknownCommand unknownCommand) {
        commandMap = new HashMap<>() {{
            put(CommandEnum.START.getName(), startCommand);
            put(CommandEnum.STOP.getName(), stopCommand);
            put(CommandEnum.INFO.getName(), infoCommand);
        }};
        defaultCommand = unknownCommand;
    }

    public void runCommand(String textCommand, MethodContainer methodContainer) {
        commandMap.getOrDefault(textCommand, defaultCommand).execute(methodContainer);
    }
}
