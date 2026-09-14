namespace ToolBook.Server.DTOs.Booking;

public class CreateBookingRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public int ToolId { get; set; }
}