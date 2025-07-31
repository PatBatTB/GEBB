create table EventMessages
(
    UserId    BIGINT  not null
        constraint PK_EventMessages
            primary key
        constraint FK_EventMessages_Users_UserId
            references Users
            on delete cascade,
    EventId   INTEGER not null,
    CreatorId BIGINT  not null,
    constraint FK_EventMessages_Events_EventId_CreatorId
        foreign key (EventId, CreatorId) references Events
            on delete cascade
);

create index IX_EventMessages_EventId_CreatorId
    on EventMessages (EventId, CreatorId);