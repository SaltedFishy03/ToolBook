using ToolBook.Server.DTOs.ToolCategory;

namespace ToolBook.Server.Services.Interfaces;

public interface IToolCategoryService
{
    Task<List<ToolCategoryResponse>> GetAllAsync();
    Task<ToolCategoryResponse?> GetByIdAsync(int id);
    Task<ToolCategoryResponse?> CreateAsync(CreateToolCategoryRequest category);
    Task<ToolCategoryResponse?> UpdateAsync(int id, UpdateToolCategoryRequest category);
    Task<bool?> DeleteAsync(int id);
}