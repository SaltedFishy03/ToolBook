using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Tool;
using ToolBook.Server.Enums;
using ToolBook.Server.Models;
using ToolBook.Server.Services;

namespace ToolBook.Server.Tests;

public class ToolServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        // Hver test får sin egen InMemory-database,
        // så testene ikke påvirker hinanden eller den rigtige database.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact] // K3 - Administration af værktøjer
    public async Task K3_ToolManagement_CreateUpdateAndProtectBookingHistory()
    {
        // Arrange
        var context = CreateContext();

        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@test.dk",
            PasswordHash = "hash"
        };

        var category = new ToolCategory
        {
            Id = 1,
            Name = "Elværktøj",
            Description = "Test kategori"
        };

        var toolType = new ToolType
        {
            Id = 1,
            Name = "Boremaskine",
            Description = "Test type",
            CategoryId = category.Id
        };

        context.Users.Add(user);
        context.ToolCategories.Add(category);
        context.ToolTypes.Add(toolType);

        await context.SaveChangesAsync();

        var service = new ToolService(context);

        // Act + Assert - opret værktøj
        var createRequest = new CreateToolRequest
        {
            ToolNumber = "BM1",
            ToolTypeId = toolType.Id,
            Status = ToolStatus.Available
        };

        var createdTool = await service.CreateAsync(createRequest);

        Assert.NotNull(createdTool);
        Assert.Equal("BM1", createdTool.ToolNumber);

        // Act + Assert - rediger værktøj
        var updateRequest = new UpdateToolRequest
        {
            ToolNumber = "BM2",
            ToolTypeId = toolType.Id,
            Status = ToolStatus.OutOfService
        };

        var updatedTool = await service.UpdateAsync(createdTool.Id, updateRequest);

        Assert.NotNull(updatedTool);
        Assert.Equal("BM2", updatedTool.ToolNumber);
        Assert.Equal(ToolStatus.OutOfService, updatedTool.Status);

        // Arrange - tilføj bookinghistorik til værktøjet
        var booking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            UserId = user.Id,
            ToolId = createdTool.Id
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act - forsøg at slette værktøjet
        var deleteResult = await service.DeleteAsync(createdTool.Id);

        // Assert - værktøj med bookinghistorik må ikke slettes
        Assert.False(deleteResult);
    }
}