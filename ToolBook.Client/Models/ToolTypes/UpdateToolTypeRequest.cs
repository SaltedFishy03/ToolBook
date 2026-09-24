namespace ToolBook.Client.Models.ToolTypes;

public class UpdateToolTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}