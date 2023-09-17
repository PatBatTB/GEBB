package com.github.patbattb.tgbot.service.message.typedispencer;

import com.github.patbattb.tgbot.service.message.command.CommandMap;
import com.github.patbattb.tgbot.container.MethodContainer;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class CommandTypeDispenser implements TypeDispenser {

    private final CommandMap commandMap;

    @Override
    public void accept(MethodContainer methodContainer) {
        String textCommand = methodContainer.getUpdate().getMessage().getText();
        commandMap.runCommand(textCommand, methodContainer);
    }
}
