using ToolBook.Server.Enums;

namespace ToolBook.Server.Models;

public class Tool
{
    public int Id { get; set; }
    
    public string ToolNumber { get; set; } = string.Empty;
    
    public ToolStatus Status { get; set; } = ToolStatus.Available;
    
    public int ToolTypeId { get; set; }
    public ToolType ToolType { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}