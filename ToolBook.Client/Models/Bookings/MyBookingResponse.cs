namespace ToolBook.Client.Models.Bookings;

public class MyBookingResponse
{
    public int Id { get; set; }
    public string ToolNumber { get; set; } = string.Empty;
    public string ToolTypeName { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? ReturnedAt { get; set; }

    public bool IsCancelled { get; set; }

    public int ToolId { get; set; }
}