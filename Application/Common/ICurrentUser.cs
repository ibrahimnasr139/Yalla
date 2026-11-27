namespace Application.Common
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        Role? Role { get; }
    }
}