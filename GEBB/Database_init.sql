Create table Users
(
    UserId bigint PRIMARY KEY ,
    Username varchar,
    RegisteredAt timestamp,
    IsActive bool
);
Create table Events
(
    EventId int PRIMARY KEY ,
    Title varchar,
    DateTimeOf timestamp,
    Address varchar,
    ParticipantLimit int,
    Cost int,
    Description varchar,
    IsActive bool
);
Create table Users_x_Events
(
    UserId bigint,
    EventId int,
    PRIMARY KEY (UserId, EventId),
    FOREIGN KEY (UserId) References Users(UserId),
    foreign key (EventId) REFERENCES Events(EventId)
);