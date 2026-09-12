using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Tool;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Services;

public class ToolService(ApplicationDbContext context) : IToolService
{
    public async Task<List<ToolResponse>> GetAllAsync()
    {
        var tools = await context.Tools
            .Select(t => new ToolResponse
            {
                Id = t.Id,
                Status = t.Status,
                ToolNumber = t.ToolNumber,
                ToolTypeId = t.ToolTypeId,
                ToolTypeName = t.ToolType.Name
            }).ToListAsync();
        return tools;
    }

    public async Task<ToolResponse?> GetByIdAsync(int id)
    {
        var tool = await context.Tools
            .Where(t => t.Id == id)
            .Select(t => new ToolResponse
            {
                Id = t.Id,
                ToolNumber = t.ToolNumber,
                Status = t.Status,
                ToolTypeId = t.ToolTypeId,
                ToolTypeName = t.ToolType.Name
            })
            .FirstOrDefaultAsync();

        return tool;
    }

    public async Task<ToolResponse?> CreateAsync(CreateToolRequest tool)
    {
        var toolNumberExists = await context.Tools.AnyAsync(t => t.ToolNumber == tool.ToolNumber);

        if (toolNumberExists)
        {
            return null;
        }
        
        var toolType = await context.ToolTypes.FindAsync(tool.ToolTypeId);

        if (toolType == null)
        {
            return null;
        }
        
        var newTool = new Tool
        {
            ToolNumber = tool.ToolNumber,
            ToolTypeId = tool.ToolTypeId
        };

        await context.Tools.AddAsync(newTool);
        await context.SaveChangesAsync();

        return new ToolResponse
        {
            Id = newTool.Id,
            ToolNumber = newTool.ToolNumber,
            Status = newTool.Status,
            ToolTypeId = newTool.ToolTypeId,
            ToolTypeName = toolType.Name
        };
    }

    public async Task<ToolResponse?> UpdateAsync(int id, UpdateToolRequest tool)
    {
        var existingTool = await context.Tools.FindAsync(id);

        if (existingTool == null)
        {
            return null;
        }

        var toolType = await context.ToolTypes.FindAsync(tool.ToolTypeId);

        if (toolType == null)
        {
            return null;
        }

        var toolNumberExists = await context.Tools.AnyAsync(t =>
            t.ToolNumber == tool.ToolNumber &&
            t.Id != id);

        if (toolNumberExists)
        {
            return null;
        }

        existingTool.Status = tool.Status;
        existingTool.ToolNumber = tool.ToolNumber;
        existingTool.ToolTypeId = tool.ToolTypeId;

        await context.SaveChangesAsync();

        return new ToolResponse
        {
            Id = existingTool.Id,
            Status = existingTool.Status,
            ToolNumber = existingTool.ToolNumber,
            ToolTypeId = existingTool.ToolTypeId,
            ToolTypeName = toolType.Name
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var toolToDelete = await context.Tools.FindAsync(id);

        if (toolToDelete == null)
        {
            return null;
        }

        var hasBookings = await context.Bookings
            .AnyAsync(b => b.ToolId == id);

        if (hasBookings)
        {
            return false;
        }

        context.Tools.Remove(toolToDelete);
        await context.SaveChangesAsync();

        return true;
    }
}