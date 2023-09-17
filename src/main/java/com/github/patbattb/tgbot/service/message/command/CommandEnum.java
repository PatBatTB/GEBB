package com.github.patbattb.tgbot.service.message.command;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

import java.util.Set;

@Getter
@RequiredArgsConstructor
public enum CommandEnum {
    START("/start", "Start command.", Set.of("new")),
    STOP("/stop", "Stop command", Set.of("user")),
    INFO("/info", "Info command", Set.of("new", "user"));

    private final String name;
    private final String description;
    private final Set<String> scopes;


}
