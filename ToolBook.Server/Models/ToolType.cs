namespace ToolBook.Server.Models;

public class ToolType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    
    public int CategoryId { get; set; }
    public ToolCategory Category { get; set; } = null!;

    public ICollection<Tool> Tools { get; set; } = new List<Tool>();
}