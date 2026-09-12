using System.ComponentModel.DataAnnotations;
using ToolBook.Server.Enums;

namespace ToolBook.Server.DTOs.Tool;

public class UpdateToolRequest
{
    [Required]
    public string ToolNumber { get; set; } = string.Empty;

    public ToolStatus Status { get; set; } = ToolStatus.Available;

    [Range(1, int.MaxValue)]
    public int ToolTypeId { get; set; }
}