using ToolBook.Server.DTOs.Tool;

namespace ToolBook.Server.Services.Interfaces;

public interface IToolService
{
    Task<List<ToolResponse>> GetAllAsync();
    Task<ToolResponse?> GetByIdAsync(int id);
    Task<ToolResponse?> CreateAsync(CreateToolRequest tool);
    Task<ToolResponse?> UpdateAsync(int id, UpdateToolRequest tool);
    Task<bool?> DeleteAsync(int id);
}