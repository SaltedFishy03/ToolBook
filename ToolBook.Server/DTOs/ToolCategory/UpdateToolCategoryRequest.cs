using System.ComponentModel.DataAnnotations;

namespace ToolBook.Server.DTOs.ToolCategory;

public class UpdateToolCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
}