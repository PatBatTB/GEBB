package com.github.patbattb.tgbot.service.message.typedispencer;

import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.service.message.MessageType;
import lombok.Value;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.Map;

@Value
@Component
public class MessageTypeMap {

    Map<MessageType, TypeDispenser> messageTypeMap;
    TypeDispenser defalutTypeDispenser;

    @Autowired
    public MessageTypeMap(CommandTypeDispenser commandTypeDispenser,
                          TextTypeDispenser textTypeDispenser,
                          UnknownTypeDispenser unknownTypeDispenser) {
        messageTypeMap = Map.of(
                MessageType.COMMAND, commandTypeDispenser,
                MessageType.TEXT, textTypeDispenser
        );
        defalutTypeDispenser = unknownTypeDispenser;
    }

    public void resolve(MessageType messageType, MethodContainer methodContainer) {
        messageTypeMap.getOrDefault(messageType, defalutTypeDispenser).accept(methodContainer);
    }

}
