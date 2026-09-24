using ToolBook.Client.Models.Tools;

namespace ToolBook.Client.Models.Bookings;

public class EditBookingViewModel
{
    public int Id { get; set; }
    public int ToolId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public List<ToolResponse> Tools { get; set; } = [];
}