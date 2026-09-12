using System.ComponentModel.DataAnnotations;

namespace ToolBook.Server.DTOs.Tool;

public class CreateToolRequest
{
    [Required]
    public string ToolNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ToolTypeId { get; set; }
}