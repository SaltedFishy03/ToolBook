using ToolBook.Server.Enums;

namespace ToolBook.Server.DTOs.Tool;

public class ToolResponse
{
    public int Id { get; set; }

    public string ToolNumber { get; set; } = string.Empty;

    public ToolStatus Status { get; set; }

    public int ToolTypeId { get; set; }

    public string ToolTypeName { get; set; } = string.Empty;
}