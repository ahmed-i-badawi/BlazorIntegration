namespace Infrastructure.ApplicationDatabase.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
}
