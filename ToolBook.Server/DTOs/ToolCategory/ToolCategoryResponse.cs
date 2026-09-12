using System.ComponentModel.DataAnnotations;

namespace ToolBook.Server.DTOs.ToolCategory;

public class ToolCategoryResponse
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}