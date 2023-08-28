package com.github.patbattb.tgbot.service.message.command;

import lombok.Getter;

@Getter
public enum CommandEnum {
    START("/start", "Start command."),
    STOP("/stop", "Stop command"),
    HELP("/help", "Help command"),
    INFO("/info", "Info command"),
    BUTTON("/button", "Button command");

    private final String name;
    private final String description;

    CommandEnum(String name, String description) {
        this.name = name;
        this.description = description;

    }


}
