namespace ToolBook.Server.Models;

public class Booking
{
    public int Id { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public DateTime? ReturnedAt { get; set; }
    
    public bool IsCancelled { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int ToolId { get; set; }
    public Tool Tool { get; set; } = null!;
}