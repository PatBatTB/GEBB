create table Alarms
(
    UserId    BIGINT  not null
        constraint FK_Alarms_Users_UserId
            references Users
            on delete cascade,
    EventId   INTEGER not null,
    CreatorId BIGINT  not null,
    LastAlert TEXT,
    constraint PK_Alarms
        primary key (UserId, EventId, CreatorId),
    constraint FK_Alarms_Events_EventId_CreatorId
        foreign key (EventId, CreatorId) references Events
            on delete cascade
);

create index IX_Alarms_EventId_CreatorId
    on Alarms (EventId, CreatorId);

create table AlarmSettings
(
    UserId    BIGINT  not null
        constraint PK_AlarmSettings
            primary key
        constraint FK_AlarmSettings_Users_UserId
            references Users
            on delete cascade,
    ThreeDays INTEGER not null,
    OneDay    INTEGER not null,
    Hours     INTEGER not null
);