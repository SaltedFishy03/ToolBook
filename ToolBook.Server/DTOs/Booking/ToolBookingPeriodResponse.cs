namespace ToolBook.Server.DTOs.Booking;

public class ToolBookingPeriodResponse
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? ReturnedAt { get; set; }
}