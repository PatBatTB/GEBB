package com.github.patbattb.tgbot.component;

import com.github.patbattb.tgbot.container.MessageContainer;
import com.github.patbattb.tgbot.service.MessageHandler;
import org.springframework.beans.factory.annotation.Autowired;
import org.telegram.telegrambots.bots.TelegramLongPollingBot;
import org.telegram.telegrambots.meta.api.methods.BotApiMethod;
import org.telegram.telegrambots.meta.api.objects.Update;
import org.telegram.telegrambots.meta.exceptions.TelegramApiException;

public class TelegramBot extends TelegramLongPollingBot {

    private final String botName;
    @Autowired
    private MessageHandler messageHandler;

    public TelegramBot(String botName, String botToken) {
        super(botToken);
        this.botName = botName;
    }

    @Override
    public void onUpdateReceived(Update update) {
        MessageContainer<? extends BotApiMethod<?>> messageContainer = messageHandler.process(update);
        try {
            execute(messageContainer.getMethod());
        } catch (TelegramApiException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public String getBotUsername() {
        return botName;
    }
}
