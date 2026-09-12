using System.ComponentModel.DataAnnotations;

namespace ToolBook.Server.DTOs.ToolType;

public class CreateToolTypeRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}