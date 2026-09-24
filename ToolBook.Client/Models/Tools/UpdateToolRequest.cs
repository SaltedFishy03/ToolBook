using ToolBook.Client.Enums;

namespace ToolBook.Client.Models.Tools;

public class UpdateToolRequest
{
    public string ToolNumber { get; set; } = string.Empty;
    public ToolStatus Status { get; set; }
    public int ToolTypeId { get; set; }
}