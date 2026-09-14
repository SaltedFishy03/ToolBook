namespace ToolBook.Server.Models;

public class Booking
{
    public int Id { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public DateOnly? ReturnedAt { get; set; }
    
    public bool IsCancelled { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int ToolId { get; set; }
    public Tool Tool { get; set; } = null!;
}