using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Models.Tools;

public class ToolOverviewViewModel
{
    public List<ToolTypeResponse> ToolTypes { get; set; } = [];
    public List<ToolTypeResponse> FilteredToolTypes { get; set; } = [];
    public List<ToolCategoryResponse> Categories { get; set; } = [];

    public int? ToolTypeId { get; set; }
    public int? CategoryId { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}