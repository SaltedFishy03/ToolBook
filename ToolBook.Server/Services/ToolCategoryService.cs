using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.ToolCategory;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Services;

public class ToolCategoryService(ApplicationDbContext context) : IToolCategoryService
{
    public async Task<List<ToolCategoryResponse>> GetAllAsync()
    {
        var categories = await context.ToolCategories
            .Select(c => new ToolCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();
        return categories;
    }

    public async Task<ToolCategoryResponse?> GetByIdAsync(int id)
    {
        var category = await context.ToolCategories.FindAsync(id);

        if (category == null)
        {
            return null;
        }

        return new ToolCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<ToolCategoryResponse?> CreateAsync(CreateToolCategoryRequest category)
    {
        var categoryExists = await context.ToolCategories.AnyAsync(t =>
            t.Name == category.Name);

        if (categoryExists)
        {
            return null;
        }
        
        var newCategory = new ToolCategory
        {
            Name = category.Name,
            Description = category.Description
        };

        await context.ToolCategories.AddAsync(newCategory);
        await context.SaveChangesAsync();
        
        var response = new ToolCategoryResponse
        {
            Id = newCategory.Id,
            Name = newCategory.Name,
            Description = newCategory.Description
        };

        return response;
    }

    public async Task<ToolCategoryResponse?> UpdateAsync(
        int id,
        UpdateToolCategoryRequest category)
    {
        var existingCategory = await context.ToolCategories.FindAsync(id);

        if (existingCategory == null)
        {
            return null;
        }

        var categoryExists = await context.ToolCategories.AnyAsync(c =>
            c.Name == category.Name &&
            c.Id != id);

        if (categoryExists)
        {
            return null;
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;

        await context.SaveChangesAsync();

        return new ToolCategoryResponse
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            Description = existingCategory.Description
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var categoryToDelete = await context.ToolCategories.FindAsync(id);

        if (categoryToDelete == null)
        {
            return null;
        }

        var hasToolTypes = await context.ToolTypes
            .AnyAsync(t => t.CategoryId == id);

        if (hasToolTypes)
        {
            return false;
        }

        context.ToolCategories.Remove(categoryToDelete);
        await context.SaveChangesAsync();

        return true;
    }
}