namespace RoomBooking.Application.Users.DTOs
{
    public sealed record AuthResponse(string Token, int ExpiresInSeconds, string Email, string Role);
}