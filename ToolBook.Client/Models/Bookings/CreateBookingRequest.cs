namespace ToolBook.Client.Models.Bookings;

public class CreateBookingRequest
{
    public int ToolId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}