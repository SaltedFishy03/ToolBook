using ToolBook.Server.DTOs.Booking;
using ToolBook.Server.Services.Results;

namespace ToolBook.Server.Services.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponse>> GetAllAsync();
    Task<List<MyBookingResponse>> GetByUserIdAsync(int userId);
    Task<BookingResponse?> GetByIdAsync(int id);

    Task<BookingResult> CreateAsync(CreateBookingRequest booking, int userId);
    Task<BookingResult> UpdateAsync(int id, UpdateBookingRequest booking, int userId);

    Task<bool?> CancelAsync(int id, int userId);
    Task<bool?> ReturnAsync(int id, int userId);
}