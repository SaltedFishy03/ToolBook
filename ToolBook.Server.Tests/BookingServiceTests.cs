using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Booking;
using ToolBook.Server.DTOs.Tool;
using ToolBook.Server.Enums;
using ToolBook.Server.Models;
using ToolBook.Server.Services;

namespace ToolBook.Server.Tests;

public class BookingServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        // Hver test får sin egen InMemory-database,
        // så testene ikke påvirker hinanden eller den rigtige database.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        return new ApplicationDbContext(options);
    }
    
    [Fact] // K2 - Et værktøj må ikke kunne dobbeltbookes
    public async Task K2_CreateAsync_WhenBookingOverlaps_ReturnsOverlap()
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

        var tool = new Tool
        {
            Id = 1,
            ToolNumber = "BM1",
            Status = ToolStatus.Available
        };

        // Eksisterende booking: dag 2-4
        var existingBooking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            UserId = user.Id,
            ToolId = tool.Id
        };

        context.Users.Add(user);
        context.Tools.Add(tool);
        context.Bookings.Add(existingBooking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Ny booking overlapper den eksisterende: dag 3-5
        var request = new CreateBookingRequest
        {
            ToolId = tool.Id,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
        };

        // Act
        var result = await service.CreateAsync(request, user.Id);

        // Assert
        Assert.Equal(BookingError.Overlap, result.Error);
    }
    
    [Fact] //K3
    public async Task K3_ToolManagement_CreateUpdateAndProtectBookingHistory()
    {
        var context = CreateContext();

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

        context.ToolCategories.Add(category);
        context.ToolTypes.Add(toolType);

        await context.SaveChangesAsync();

        var service = new ToolService(context);

        var createRequest = new CreateToolRequest
        {
            ToolNumber = "BM1",
            ToolTypeId = toolType.Id,
            Status = ToolStatus.Available
        };

        var createdTool = await service.CreateAsync(createRequest);

        Assert.NotNull(createdTool);
        Assert.Equal("BM1", createdTool.ToolNumber);

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

        var booking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            UserId = 1,
            ToolId = createdTool.Id
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var deleteResult = await service.DeleteAsync(createdTool.Id);

        Assert.False(deleteResult);
    }
    
    [Fact] //K9
    public async Task K9_CreateBooking_WithInvalidDates_IsRejected()
    {
        var context = CreateContext();

        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@test.dk",
            PasswordHash = "hash"
        };

        var tool = new Tool
        {
            Id = 1,
            ToolNumber = "BM1",
            Status = ToolStatus.Available
        };

        context.Users.Add(user);
        context.Tools.Add(tool);
        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var request = new CreateBookingRequest
        {
            ToolId = tool.Id,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        };

        var result = await service.CreateAsync(request, user.Id);

        Assert.Equal(BookingError.InvalidDate, result.Error);
        Assert.Empty(context.Bookings);
    }
    
    [Fact] // K11 - Fremtidig booking kan redigeres
    public async Task K11_UpdateFutureBooking_ChangesToolAndDates()
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

        var firstTool = new Tool
        {
            Id = 1,
            ToolNumber = "BM1",
            Status = ToolStatus.Available
        };

        var secondTool = new Tool
        {
            Id = 2,
            ToolNumber = "BM2",
            Status = ToolStatus.Available
        };

        var booking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            UserId = user.Id,
            ToolId = firstTool.Id
        };

        context.Users.Add(user);
        context.Tools.AddRange(firstTool, secondTool);
        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var request = new UpdateBookingRequest
        {
            ToolId = secondTool.Id,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
        };

        // Act
        var result = await service.UpdateAsync(booking.Id, request, user.Id);

        // Assert
        Assert.Equal(BookingError.None, result.Error);

        var updatedBooking = await context.Bookings.FindAsync(booking.Id);

        Assert.NotNull(updatedBooking);
        Assert.Equal(secondTool.Id, updatedBooking.ToolId);
        Assert.Equal(request.StartDate, updatedBooking.StartDate);
        Assert.Equal(request.EndDate, updatedBooking.EndDate);
    }
    
    [Fact] // K11 - Fremtidig booking kan annulleres
    public async Task K11_CancelFutureBooking_SetsIsCancelled()
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

        var tool = new Tool
        {
            Id = 1,
            ToolNumber = "BM1",
            Status = ToolStatus.Available
        };

        var booking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            UserId = user.Id,
            ToolId = tool.Id
        };

        context.Users.Add(user);
        context.Tools.Add(tool);
        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.CancelAsync(booking.Id, user.Id);

        // Assert
        Assert.True(result.HasValue && result.Value);

        var cancelledBooking = await context.Bookings.FindAsync(booking.Id);

        Assert.NotNull(cancelledBooking);
        Assert.True(cancelledBooking.IsCancelled);
    }
    
    [Fact] // K11 - Aktiv booking kan registreres som returneret
    public async Task K11_ReturnActiveBooking_SetsReturnedAt()
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

        var tool = new Tool
        {
            Id = 1,
            ToolNumber = "BM1",
            Status = ToolStatus.Available
        };

        var booking = new Booking
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            UserId = user.Id,
            ToolId = tool.Id
        };

        context.Users.Add(user);
        context.Tools.Add(tool);
        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.ReturnAsync(booking.Id, user.Id);

        // Assert
        Assert.True(result.HasValue && result.Value);

        var returnedBooking = await context.Bookings.FindAsync(booking.Id);

        Assert.NotNull(returnedBooking);
        Assert.Equal(
            DateOnly.FromDateTime(DateTime.Today),
            returnedBooking.ReturnedAt);
    }
}