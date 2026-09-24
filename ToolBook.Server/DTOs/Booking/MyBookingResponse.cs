namespace ToolBook.Server.DTOs.Booking;

public class MyBookingResponse
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? ReturnedAt { get; set; }

    public bool IsCancelled { get; set; }

    public int UserId { get; set; }
    public int ToolId { get; set; }
    
    public string ToolNumber { get; set; } = string.Empty;
    public string ToolTypeName { get; set; } = string.Empty;
}