package com.github.patbattb.tgbot.service;

import com.github.patbattb.tgbot.service.keyboard.KeyboardService;
import com.github.patbattb.tgbot.service.message.MessageType;
import com.github.patbattb.tgbot.service.message.MessageTypeMap;
import com.github.patbattb.tgbot.container.MessageContainer;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.BotApiMethod;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;
import org.telegram.telegrambots.meta.api.methods.updatingmessages.EditMessageText;
import org.telegram.telegrambots.meta.api.objects.Update;


@Service
public class MessageHandler {

    private final MessageParser messageParser;
    private final MessageTypeMap messageTypeMap;

    @Autowired
    public MessageHandler(MessageParser messageParser, MessageTypeMap messageTypeMap) {
        this.messageParser = messageParser;
        this.messageTypeMap = messageTypeMap;
    }

    public MessageContainer<? extends BotApiMethod<?>> process(Update update) {
        if (update.hasMessage()) return processSendMessage(update);
        if (update.hasCallbackQuery()) return processEditMessage(update);
        return null;
    }

    private MessageContainer<SendMessage> processSendMessage(Update update) {
        MessageContainer<SendMessage> messageContainer = new MessageContainer<>(update, new SendMessage());
        messageContainer.getMethod().setChatId(messageContainer.getUpdate().getMessage().getChatId());
        MessageType type = messageParser.getType(messageContainer);
        messageTypeMap.resolve(type, messageContainer);
        return messageContainer;
    }

    private MessageContainer<EditMessageText> processEditMessage(Update update) {
        int messageId = update.getCallbackQuery().getMessage().getMessageId();
        long chatId = update.getCallbackQuery().getMessage().getChatId();
        String queryData = update.getCallbackQuery().getData();
        EditMessageText newMessage = new EditMessageText();
        newMessage.setChatId(chatId);
        newMessage.setMessageId(messageId);
        if ("BUTTON_1_YES".equals(queryData) || "BUTTON_2_YES".equals(queryData)) {
            newMessage.setText("Супер!");
        } else if ("BUTTON_1_NO".equals(queryData)) {
            newMessage.setText("А сейчас?");
            String mask = "BUTTON_2";
            newMessage.setReplyMarkup(KeyboardService.getYesNoKeyboard(mask));
        } else if ("BUTTON_2_NO".equals(queryData)) {
            newMessage.setText("Обидно! =((");
        }
        return new MessageContainer<>(update, newMessage);
    }

}
