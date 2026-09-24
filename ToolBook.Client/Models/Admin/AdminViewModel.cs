using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.Tools;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Models.Admin;

public class AdminViewModel
{
    public List<ToolResponse> Tools { get; set; } = new();
    public List<ToolTypeResponse> ToolTypes { get; set; } = new();
    public List<ToolCategoryResponse> Categories { get; set; } = new();
}