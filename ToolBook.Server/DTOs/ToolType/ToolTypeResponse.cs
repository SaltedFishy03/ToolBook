using System.ComponentModel.DataAnnotations;

namespace ToolBook.Server.DTOs.ToolType;

public class ToolTypeResponse
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    
    public string CategoryName { get; set; } = string.Empty;
}