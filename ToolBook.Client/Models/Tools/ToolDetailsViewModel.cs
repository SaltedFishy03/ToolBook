namespace ToolBook.Client.Models.Tools;

public class ToolDetailsViewModel
{
    public List<ToolDetailsResponse> Tools { get; set; } = [];

    public int ToolTypeId { get; set; }
    public string ToolTypeName { get; set; } = string.Empty;

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}