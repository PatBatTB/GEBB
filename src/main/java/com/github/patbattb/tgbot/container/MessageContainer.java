package com.github.patbattb.tgbot.container;

import lombok.Value;
import org.telegram.telegrambots.meta.api.methods.BotApiMethod;
import org.telegram.telegrambots.meta.api.objects.Update;


@Value
public class MessageContainer<T extends BotApiMethod<?>> {
    Update update;
    T method;
}
