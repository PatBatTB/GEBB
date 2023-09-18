package com.github.patbattb.tgbot.service.keyboard;

import lombok.experimental.UtilityClass;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.InlineKeyboardMarkup;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.ReplyKeyboardMarkup;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.buttons.InlineKeyboardButton;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.buttons.KeyboardButton;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.buttons.KeyboardRow;

import java.util.List;

@UtilityClass
public class KeyboardService {

    public InlineKeyboardMarkup getYesNoKeyboard(String mask) {
        var markup = new InlineKeyboardMarkup();
        var buttonYes = new InlineKeyboardButton("Да");
        buttonYes.setCallbackData(mask + "_YES");
        var buttonNo = new InlineKeyboardButton("Нет");
        buttonNo.setCallbackData(mask + "_NO");
        markup.setKeyboard(List.of(List.of(buttonYes, buttonNo)));
        return markup;
    }

    /* replyKeyBoard добавляется к sendMessage (Что бы удалить ее у клиента - нужно отредактировать сообщение),
    либо что бы пользователь удалил сообщение у себя.
     */
    public ReplyKeyboardMarkup getDefaultKeyBoard() {
        var markup = new ReplyKeyboardMarkup();
        var keyboard = List.of(
                new KeyboardRow(
                        List.of(new KeyboardButton("Yes"), new KeyboardButton("No"))));
        markup.setKeyboard(keyboard);
        markup.setResizeKeyboard(true);
        markup.setOneTimeKeyboard(true);
        return  markup;
    }
}
