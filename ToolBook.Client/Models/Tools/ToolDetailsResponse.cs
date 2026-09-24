using ToolBook.Client.Enums;

namespace ToolBook.Client.Models.Tools;

public class ToolDetailsResponse
{
    public int Id { get; set; }
    public string ToolNumber { get; set; } = string.Empty;
    public ToolStatus Status { get; set; }
    public int ToolTypeId { get; set; }
    public string ToolTypeName { get; set; } = string.Empty;
    public bool? IsAvailable { get; set; }
    public List<ToolBookingPeriodResponse> Bookings { get; set; } = [];
}