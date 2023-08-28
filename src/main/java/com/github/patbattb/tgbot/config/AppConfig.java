package com.github.patbattb.tgbot.config;

import com.github.patbattb.tgbot.component.BotMenu;
import com.github.patbattb.tgbot.component.TelegramBot;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.telegram.telegrambots.meta.TelegramBotsApi;
import org.telegram.telegrambots.meta.api.methods.commands.SetMyCommands;
import org.telegram.telegrambots.meta.api.objects.commands.scope.BotCommandScopeDefault;
import org.telegram.telegrambots.meta.exceptions.TelegramApiException;
import org.telegram.telegrambots.updatesreceivers.DefaultBotSession;

@Slf4j
@Configuration
public class AppConfig {

    @Value("${bot.name}")
    String botName;

    @Value("${bot.token}")
    String botToken;

    @Bean
    public TelegramBot telegramBot() {
        TelegramBot bot = new TelegramBot(botName, botToken);
        try {
            bot.execute(new SetMyCommands(botMenu().getListOfCommands(), new BotCommandScopeDefault(), null));
        } catch (TelegramApiException e) {
            throw new RuntimeException(e);
        }
        return bot;
    }

    @Bean
    public TelegramBotsApi telegramBotsApi(TelegramBot telegramBot) throws TelegramApiException {
        TelegramBotsApi api = new TelegramBotsApi(DefaultBotSession.class);
        api.registerBot(telegramBot);
        log.info("Telegram bot is registered.");
        return api;
    }

    @Bean
    public BotMenu botMenu() {
        return new BotMenu();
    }
}
