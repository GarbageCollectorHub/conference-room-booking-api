namespace RoomBooking.Application.Reports.DTOs
{
    public sealed record DailyReportItem(
        DateOnly Date, 
        int BookingsCount, 
        decimal Revenue
        );
}