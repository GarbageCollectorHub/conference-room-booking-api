namespace RoomBooking.Application.Reports.DTOs
{
    public sealed record RoomReportItem(
        Guid RoomId,
        string RoomName,
        int BookingsCount,
        decimal Revenue
        );

}