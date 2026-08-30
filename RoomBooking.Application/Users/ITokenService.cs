using RoomBooking.Domain.Users;

namespace RoomBooking.Application.Users
{
    public interface ITokenService
    {
        (string Token, int ExpiresInSeconds) CreateAccessToken(User user);
    }
}