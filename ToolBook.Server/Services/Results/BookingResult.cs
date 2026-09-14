using ToolBook.Server.DTOs.Booking;
using ToolBook.Server.Enums;

namespace ToolBook.Server.Services.Results;

public class BookingResult
{
    public BookingResponse? Booking { get; set; }
    public BookingError Error { get; set; }

    public bool Success => Error == BookingError.None;
}