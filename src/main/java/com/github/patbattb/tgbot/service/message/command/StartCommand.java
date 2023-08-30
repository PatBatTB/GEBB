package com.github.patbattb.tgbot.service.message.command;

import com.github.patbattb.tgbot.container.MessageContainer;
import com.github.patbattb.tgbot.model.User;
import com.github.patbattb.tgbot.model.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

import java.sql.Timestamp;
import java.time.LocalDateTime;
import java.util.Optional;

@Service
public class StartCommand implements Command {

    @Autowired
    UserRepository userRepository;

    @Override
    public void execute(MessageContainer<SendMessage> messageContainer) {
        messageContainer.getMethod().setText("Hi, you run start command.");
        var chatId = messageContainer.getUpdate().getMessage().getChatId();
        String username = messageContainer.getUpdate().getMessage().getChat().getUserName();
        Optional<User> optional = userRepository.findById(chatId);
        if (optional.isEmpty()) {
            optional = Optional.of(User.builder()
                    .id(chatId)
                    .name(username)
                    .active(true)
                    .registerDate(Timestamp.valueOf(LocalDateTime.now()))
                    .build());
            messageContainer.getMethod().setText("New user created");
        } else {
            User user = optional.get().toBuilder()
                    .name(username)
                    .build();
            optional = Optional.of(user);
            messageContainer.getMethod().setText("User updated");
        }
        userRepository.save(optional.get());
    }
}
