using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Models.Admin;

public class EditToolTypeViewModel
{
    public UpdateToolTypeRequest ToolType { get; set; } = new();
    public List<ToolCategoryResponse> Categories { get; set; } = new();
}