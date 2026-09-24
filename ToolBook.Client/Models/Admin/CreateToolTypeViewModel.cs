using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Models.Admin;

public class CreateToolTypeViewModel
{
    public CreateToolTypeRequest ToolType { get; set; } = new();
    public List<ToolCategoryResponse> Categories { get; set; } = new();
}