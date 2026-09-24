using ToolBook.Client.Models.Tools;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Models.Admin;

public class CreateToolViewModel
{
    public CreateToolRequest Tool { get; set; } = new();
    public List<ToolTypeResponse> ToolTypes { get; set; } = new();
}