using ToolBook.Server.DTOs.ToolType;

namespace ToolBook.Server.Services.Interfaces;

public interface IToolTypeService
{
    Task<List<ToolTypeResponse>> GetAllAsync();
    Task<ToolTypeResponse?> GetByIdAsync(int id);
    Task<ToolTypeResponse?> CreateAsync(CreateToolTypeRequest toolType);
    Task<ToolTypeResponse?> UpdateAsync(int id, UpdateToolTypeRequest toolType);
    Task<bool?> DeleteAsync(int id);
}