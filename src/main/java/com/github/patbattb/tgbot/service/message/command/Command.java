package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MethodContainer;

@FunctionalInterface
public interface Command {
    void execute(MethodContainer methodContainer);
}
