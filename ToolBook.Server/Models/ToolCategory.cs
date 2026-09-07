namespace ToolBook.Server.Models;

public class ToolCategory
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<ToolType> ToolTypes { get; set; } = new List<ToolType>();
}