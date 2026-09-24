namespace ToolBook.Client.Models.Bookings;

public class UpdateBookingRequest
{
    public int ToolId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}