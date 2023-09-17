package com.github.patbattb.tgbot.service;

import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.service.message.typedispencer.MessageTypeMap;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.BotApiMethod;
import org.telegram.telegrambots.meta.api.objects.Update;

import java.util.List;


@Service
@RequiredArgsConstructor
public class MessageHandler {

    private final MessageTypeMap messageTypeMap;
    private final UserParser userParser;

    public List<BotApiMethod<?>> process(Update update) {
        var methodContainer = new MethodContainer(update);
        userParser.parse(methodContainer);
        if (update.hasMessage()) processMessage(methodContainer);
        return methodContainer.getMethods();
    }

    private void processMessage(MethodContainer methodContainer) {
        var type = MessageParser.getType(methodContainer.getUpdate());
        messageTypeMap.resolve(type, methodContainer);
    }
}
