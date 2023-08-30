package com.github.patbattb.tgbot.model;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import lombok.Builder;
import lombok.Getter;

import java.sql.Timestamp;

@Entity(name="users")
@Getter
public class User {

    @Builder(toBuilder=true)
    public User(Long id, String name, boolean active, Timestamp registerDate) {
        this.id = id;
        this.name = name;
        this.active = active;
        this.registerDate = registerDate;
    }

    public User() { } //for hibernate


    @Id
    private Long id;
    private String name;
    private boolean active;
    private Timestamp registerDate;

}
