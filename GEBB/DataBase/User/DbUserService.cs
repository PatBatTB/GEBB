using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain.Enums;

namespace Com.Github.PatBatTB.GEBB.DataBase.User;

public class DbUserService : IUserService
{
    public UserDto Merge(Telegram.Bot.Types.User tgUser)
    {
        using TgBotDbContext db = new();
        var user = db.Find<UserEntity>(tgUser.Id);
        if (user is not null)
        {
            user.Username = tgUser.Username;
        }
        else
        {
            user = new UserEntity
            {
                UserId = tgUser.Id,
                Username = tgUser.Username,
                UserStatus = UserStatus.Newuser,
                RegisteredAt = DateTime.Now
            };
            db.Add(user);
        }

        db.SaveChanges();
        return EntityToDto(user);
    }

    public void Merge(UserDto user)
    {
        using TgBotDbContext db = new();
        db.Update(DtoToEntity(user));
        db.SaveChanges();
    }

    public void Remove(UserDto user)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a list of all active users from the database, excluding the creator, to be invited to the event.
    /// </summary>
    /// <param name="entity">Event</param>
    /// <returns>List of usersID</returns>
    public ICollection<UserDto> GetInviteList(EventDto eventDto)
    {
        using TgBotDbContext db = new();
        ICollection<UserEntity> entities = db.Users
            .Where(user => user.UserStatus != UserStatus.Stop) // &&
            //user.UserId != entity.CreatorId) //TODO Debug (отправляю себе же приглашение для отладки)
            .ToList();
        return entities.Select(EntityToDto).ToList();
    }

    public UserDto EntityToDto(UserEntity entity)
    {
        return new()
        {
            UserId = entity.UserId,
            Username = entity.Username,
            UserStatus = entity.UserStatus,
            RegisteredAt = entity.RegisteredAt,
        };
    }

    public UserEntity DtoToEntity(UserDto dto)
    {
        return new()
        {
            UserId = dto.UserId,
            Username = dto.Username,
            UserStatus = dto.UserStatus,
            RegisteredAt = dto.RegisteredAt
        };
    }
}