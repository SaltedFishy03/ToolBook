using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.ToolType;
using ToolBook.Server.Enums;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Services;

public class ToolTypeService(ApplicationDbContext context) : IToolTypeService
{
    public async Task<List<ToolTypeResponse>> GetAllAsync()
    {
        var toolTypes = await context.ToolTypes
            .Select(t => new ToolTypeResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            }).ToListAsync();
        return toolTypes;
    }

    public async Task<ToolTypeResponse?> GetByIdAsync(int id)
    {
        var toolType = await context.ToolTypes
            .Where(t => t.Id == id)
            .Select(t => new ToolTypeResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            }).FirstOrDefaultAsync();

        return toolType;
    }

    private bool IsToolAvailable(Tool tool, DateOnly startDate, DateOnly endDate)
    {
        if (tool.Status != ToolStatus.Available)
        {
            return false;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        foreach (var booking in tool.Bookings)
        {
            if (booking.IsCancelled)
            {
                continue;
            }

            if (!booking.ReturnedAt.HasValue && booking.EndDate < today)
            {
                return false;
            }

            var bookingEnd = booking.ReturnedAt ?? booking.EndDate;

            var overlaps =
                booking.StartDate <= endDate &&
                bookingEnd >= startDate;

            if (overlaps)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<List<ToolTypeResponse>> GetFilteredAsync(int? toolTypeId, int? categoryId, DateOnly? startDate, DateOnly? endDate)
    {
        var toolTypes = await context.ToolTypes
            .Include(tt => tt.Category)
            .Include(tt => tt.Tools)
            .ThenInclude(t => t.Bookings)
            .ToListAsync();

        if (toolTypeId.HasValue)
        {
            toolTypes = toolTypes
                .Where(tt => tt.Id == toolTypeId.Value)
                .ToList();
        }

        if (categoryId.HasValue)
        {
            toolTypes = toolTypes
                .Where(tt => tt.CategoryId == categoryId.Value)
                .ToList();
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            toolTypes = toolTypes
                .Where(tt => tt.Tools.Any(tool =>
                    IsToolAvailable(tool, startDate.Value, endDate.Value)))
                .ToList();
        }

        return toolTypes.Select(tt => new ToolTypeResponse
        {
            Id = tt.Id,
            Name = tt.Name,
            Description = tt.Description,
            CategoryId = tt.CategoryId,
            CategoryName = tt.Category.Name
        }).ToList();
    }

    public async Task<ToolTypeResponse?> CreateAsync(CreateToolTypeRequest toolType)
    {
        var toolCategory = await context.ToolCategories.FindAsync(toolType.CategoryId);

        if (toolCategory == null)
        {
            return null;
        }

        var toolTypeExists = await context.ToolTypes.AnyAsync(t =>
            t.Name == toolType.Name &&
            t.CategoryId == toolType.CategoryId);

        if (toolTypeExists)
        {
            return null;
        }

        var newToolType = new ToolType
        {
            Name = toolType.Name,
            Description = toolType.Description,
            CategoryId = toolType.CategoryId
        };

        await context.ToolTypes.AddAsync(newToolType);
        await context.SaveChangesAsync();

        return new ToolTypeResponse
        {
            Id = newToolType.Id,
            Name = newToolType.Name,
            Description = newToolType.Description,
            CategoryId = newToolType.CategoryId,
            CategoryName = toolCategory.Name
        };
    }

    public async Task<ToolTypeResponse?> UpdateAsync(int id, UpdateToolTypeRequest toolType)
    {
        var existingToolType = await context.ToolTypes.FindAsync(id);

        if (existingToolType == null)
        {
            return null;
        }

        var toolCategory = await context.ToolCategories.FindAsync(toolType.CategoryId);

        if (toolCategory == null)
        {
            return null;
        }

        var toolTypeExists = await context.ToolTypes.AnyAsync(t =>
            t.Name == toolType.Name &&
            t.CategoryId == toolType.CategoryId &&
            t.Id != id);

        if (toolTypeExists)
        {
            return null;
        }

        existingToolType.Name = toolType.Name;
        existingToolType.Description = toolType.Description;
        existingToolType.CategoryId = toolType.CategoryId;
        await context.SaveChangesAsync();

        return new ToolTypeResponse
        {
            Id = existingToolType.Id,
            Name = existingToolType.Name,
            Description = existingToolType.Description,
            CategoryId = existingToolType.CategoryId,
            CategoryName = toolCategory.Name
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var toolTypeToDelete = await context.ToolTypes.FindAsync(id);

        if (toolTypeToDelete == null)
        {
            return null;
        }

        var hasTools = await context.Tools
            .AnyAsync(t => t.ToolTypeId == id);

        if (hasTools)
        {
            return false;
        }

        context.ToolTypes.Remove(toolTypeToDelete);
        await context.SaveChangesAsync();

        return true;
    }
}