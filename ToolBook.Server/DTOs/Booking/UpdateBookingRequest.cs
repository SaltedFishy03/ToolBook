namespace ToolBook.Server.DTOs.Booking;

public class UpdateBookingRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public int ToolId { get; set; }
}