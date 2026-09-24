namespace ToolBook.Client.Models.ToolTypes;

public class CreateToolTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}