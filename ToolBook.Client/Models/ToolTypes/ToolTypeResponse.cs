namespace ToolBook.Client.Models.ToolTypes;
public class ToolTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}