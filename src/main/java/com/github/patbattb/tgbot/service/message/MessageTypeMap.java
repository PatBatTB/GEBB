package com.github.patbattb.tgbot.service.message;

import com.github.patbattb.tgbot.container.MessageContainer;
import lombok.Value;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

import java.util.HashMap;
import java.util.Map;

@Value
@Component
public class MessageTypeMap {

    Map<MessageType, TypeDispenser> messageTypeMap;
    TypeDispenser defalutTypeDispenser;

    @Autowired
    public MessageTypeMap(CommandTypeDispenser commandTypeDispenser, TextTypeDispenser textTypeDispenser,
                          UnknownTypeDispenser unknownTypeDispenser) {
        messageTypeMap = new HashMap<>() {{
            put(MessageType.COMMAND, commandTypeDispenser);
            put(MessageType.TEXT, textTypeDispenser);
        }};
        defalutTypeDispenser = unknownTypeDispenser;
    }

    public void resolve(MessageType messageType, MessageContainer<SendMessage> messageContainer) {
        messageTypeMap.getOrDefault(messageType, defalutTypeDispenser).accept(messageContainer);
    }

}
