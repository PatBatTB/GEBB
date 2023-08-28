package com.github.patbattb.tgbot.service.keyboard;

import lombok.experimental.UtilityClass;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.InlineKeyboardMarkup;
import org.telegram.telegrambots.meta.api.objects.replykeyboard.buttons.InlineKeyboardButton;

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
}
