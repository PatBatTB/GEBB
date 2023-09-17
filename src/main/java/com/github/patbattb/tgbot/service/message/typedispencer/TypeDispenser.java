package com.github.patbattb.tgbot.service.message.typedispencer;

import com.github.patbattb.tgbot.container.MethodContainer;

@FunctionalInterface
public interface TypeDispenser {
    void accept(MethodContainer methodContainer);
}
