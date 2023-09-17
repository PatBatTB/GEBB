package com.github.patbattb.tgbot.container;

import com.github.patbattb.tgbot.model.User;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;
import org.telegram.telegrambots.meta.api.methods.BotApiMethod;
import org.telegram.telegrambots.meta.api.objects.Update;

import java.util.ArrayList;
import java.util.List;


@Getter
@RequiredArgsConstructor
public class MethodContainer {
    private final Update update;
    @Setter
    private User user;
    private final List<BotApiMethod<?>> methods = new ArrayList<>();

}
