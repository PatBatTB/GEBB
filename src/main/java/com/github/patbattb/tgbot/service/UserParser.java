package com.github.patbattb.tgbot.service;

import com.github.patbattb.tgbot.container.MethodContainer;
import com.github.patbattb.tgbot.model.User;
import com.github.patbattb.tgbot.model.UserRepository;
import lombok.Value;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.sql.Timestamp;
import java.time.LocalDateTime;
import java.util.Optional;

@Service
@Value
@Slf4j
public class UserParser {

    UserRepository userRepository;

    public void parse(MethodContainer methodContainer) {
        var chatId = methodContainer.getUpdate().getMessage().getChatId();
        var username = methodContainer.getUpdate().getMessage().getChat().getUserName();
        var optional = userRepository.findById(chatId);
        if (optional.isEmpty()) {
            optional = Optional.of(User.builder()
                    .id(chatId)
                    .name(username)
                    .active(false)
                    .registerDate(Timestamp.valueOf(LocalDateTime.now()))
                    .build());
            log.debug(String.format("User %s : %s --- added to DB.",
                    methodContainer.getUpdate().getMessage().getChatId(),
                    methodContainer.getUpdate().getMessage().getChat().getUserName()));
        } else {
            var user = optional.get().toBuilder()
                    .name(username)
                    .build();
            optional = Optional.of(user);
        }
        userRepository.save(optional.get());
        methodContainer.setUser(optional.get());
    }
}
