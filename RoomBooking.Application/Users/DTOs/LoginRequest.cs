namespace RoomBooking.Application.Users.DTOs
{
    public sealed record LoginRequest(string Email, string Password);
}