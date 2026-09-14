using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Booking;
using ToolBook.Server.Enums;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;
using ToolBook.Server.Services.Results;

namespace ToolBook.Server.Services;

public class BookingService(ApplicationDbContext context) : IBookingService
{
    public async Task<List<BookingResponse>> GetAllAsync()
    {
        var bookings = await context.Bookings
            .Select(b => new BookingResponse
            {
                Id = b.Id,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                ReturnedAt = b.ReturnedAt,
                IsCancelled = b.IsCancelled,
                UserId = b.UserId,
                ToolId = b.ToolId
            })
            .ToListAsync();

        return bookings;
    }

    public async Task<List<BookingResponse>> GetByUserIdAsync(int userId)
    {
        var bookings = await context.Bookings
            .Where(b => b.UserId == userId)
            .Select(b => new BookingResponse
            {
                Id = b.Id,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                ReturnedAt = b.ReturnedAt,
                IsCancelled = b.IsCancelled,
                UserId = b.UserId,
                ToolId = b.ToolId
            })
            .ToListAsync();

        return bookings;
    }

    public async Task<BookingResponse?> GetByIdAsync(int id)
    {
        var booking = await context.Bookings
            .Where(b => b.Id == id)
            .Select(b => new BookingResponse
            {
                Id = b.Id,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                ReturnedAt = b.ReturnedAt,
                IsCancelled = b.IsCancelled,
                UserId = b.UserId,
                ToolId = b.ToolId
            })
            .FirstOrDefaultAsync();

        return booking;
    }

    public async Task<BookingResult> CreateAsync(CreateBookingRequest booking, int userId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        // En booking må ikke starte i fortiden,
        // og slutdatoen må ikke ligge før startdatoen.
        if (booking.StartDate < today || booking.EndDate < booking.StartDate)
        {
            return new BookingResult
            {
                Error = BookingError.InvalidDate
            };
        }

        var tool = await context.Tools.FindAsync(booking.ToolId);

        if (tool == null)
        {
            return new BookingResult
            {
                Error = BookingError.ToolNotFound
            };
        }

        // Kun værktøjer med status Available må bookes.
        // Maintenance og OutOfService skal derfor afvises.
        if (tool.Status != ToolStatus.Available)
        {
            return new BookingResult
            {
                Error = BookingError.ToolUnavailable
            };
        }

        // Kontrollerer om værktøjet allerede er optaget
        // i hele eller dele af den ønskede periode.
        var hasOverlap = await HasOverlapAsync(booking.ToolId, booking.StartDate, booking.EndDate);

        if (hasOverlap)
        {
            return new BookingResult
            {
                Error = BookingError.Overlap
            };
        }

        var userExists = await context.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return new BookingResult
            {
                Error = BookingError.UserNotFound
            };
        }

        // UserId kommer fra den autentificerede bruger
        // og bliver ikke sendt ind via CreateBookingRequest.
        var newBooking = new Booking
        {
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            ReturnedAt = null,
            IsCancelled = false,
            UserId = userId,
            ToolId = booking.ToolId
        };

        await context.Bookings.AddAsync(newBooking);
        await context.SaveChangesAsync();

        var response = new BookingResponse
        {
            Id = newBooking.Id,
            StartDate = newBooking.StartDate,
            EndDate = newBooking.EndDate,
            ReturnedAt = newBooking.ReturnedAt,
            IsCancelled = newBooking.IsCancelled,
            UserId = newBooking.UserId,
            ToolId = newBooking.ToolId
        };

        return new BookingResult
        {
            Booking = response,
            Error = BookingError.None
        };
    }

    public async Task<BookingResult> UpdateAsync(int id, UpdateBookingRequest booking, int userId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Brugeren må kun ændre sine egne bookinger.
        var existingBooking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (existingBooking == null)
        {
            return new BookingResult
            {
                Error = BookingError.BookingNotFound
            };
        }

        // En annulleret eller afleveret booking kan ikke længere ændres.
        if (existingBooking.IsCancelled || existingBooking.ReturnedAt != null)
        {
            return new BookingResult
            {
                Error = BookingError.BookingNotEditable
            };
        }

        var bookingHasStarted = existingBooking.StartDate <= today;

        if (bookingHasStarted)
        {
            // Når bookingen er startet, må kun slutdatoen ændres.
            // Startdato og værktøj skal derfor være de samme som før.
            if (booking.StartDate != existingBooking.StartDate ||
                booking.ToolId != existingBooking.ToolId)
            {
                return new BookingResult
                {
                    Error = BookingError.BookingNotEditable
                };
            }

            // Slutdatoen må ikke flyttes tilbage i fortiden.
            if (booking.EndDate < today)
            {
                return new BookingResult
                {
                    Error = BookingError.InvalidDate
                };
            }

            // Ved Update ignoreres bookingen selv i overlapkontrollen.
            var hasOverlap = await HasOverlapAsync(
                existingBooking.ToolId,
                existingBooking.StartDate,
                booking.EndDate,
                id);

            if (hasOverlap)
            {
                return new BookingResult
                {
                    Error = BookingError.Overlap
                };
            }

            // På en igangværende booking ændres kun slutdatoen.
            existingBooking.EndDate = booking.EndDate;
        }
        else
        {
            // På en fremtidig booking må både datoer og værktøj ændres.
            if (booking.StartDate < today || booking.EndDate < booking.StartDate)
            {
                return new BookingResult
                {
                    Error = BookingError.InvalidDate
                };
            }

            var tool = await context.Tools.FindAsync(booking.ToolId);

            if (tool == null)
            {
                return new BookingResult
                {
                    Error = BookingError.ToolNotFound
                };
            }

            if (tool.Status != ToolStatus.Available)
            {
                return new BookingResult
                {
                    Error = BookingError.ToolUnavailable
                };
            }

            // Ved Update ignoreres bookingen selv i overlapkontrollen.
            var hasOverlap = await HasOverlapAsync(
                booking.ToolId,
                booking.StartDate,
                booking.EndDate,
                id);

            if (hasOverlap)
            {
                return new BookingResult
                {
                    Error = BookingError.Overlap
                };
            }

            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.ToolId = booking.ToolId;
        }

        await context.SaveChangesAsync();

        var response = new BookingResponse
        {
            Id = existingBooking.Id,
            StartDate = existingBooking.StartDate,
            EndDate = existingBooking.EndDate,
            ReturnedAt = existingBooking.ReturnedAt,
            IsCancelled = existingBooking.IsCancelled,
            UserId = existingBooking.UserId,
            ToolId = existingBooking.ToolId
        };

        return new BookingResult
        {
            Booking = response,
            Error = BookingError.None
        };
    }


    public async Task<bool?> CancelAsync(int id, int userId)
    {
        // Brugeren må kun annullere sine egne bookinger.
        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        // En booking kan kun annulleres før den starter.
        // Når den først er startet, bruges Return i stedet.
        if (booking.IsCancelled || booking.ReturnedAt != null || booking.StartDate <= today)
        {
            return false;
        }

        // Bookingen slettes ikke fra databasen,
        // så historikken bliver bevaret.
        booking.IsCancelled = true;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool?> ReturnAsync(int id, int userId)
    {
        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (booking.IsCancelled || booking.ReturnedAt != null || booking.StartDate > today)
        {
            return false;
        }

        booking.ReturnedAt = today;

        await context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> HasOverlapAsync(int toolId, DateOnly startDate, DateOnly endDate,
        int? excludeBookingId = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var bookings = await context.Bookings
            .Where(b => b.ToolId == toolId && !b.IsCancelled)
            .ToListAsync();

        foreach (var existingBooking in bookings)
        {
            // Bruges ved Update, så bookingen ikke overlapper med sig selv.
            if (excludeBookingId.HasValue &&
                existingBooking.Id == excludeBookingId.Value)
            {
                continue;
            }

            // Hvis værktøjet allerede er afleveret, bruges ReturnedAt
            // som den faktiske slutdato for bookingen.
            if (existingBooking.ReturnedAt.HasValue)
            {
                var returnedAt = existingBooking.ReturnedAt.Value;

                if (existingBooking.StartDate <= endDate &&
                    returnedAt >= startDate)
                {
                    return true;
                }

                continue;
            }

            // Hvis EndDate er overskredet, men værktøjet ikke er afleveret,
            // betragtes værktøjet stadig som udlånt.
            if (existingBooking.EndDate < today)
            {
                return true;
            }

            // Normal overlapkontrol mellem to bookingperioder.
            var hasOverlap =
                existingBooking.StartDate <= endDate &&
                existingBooking.EndDate >= startDate;

            if (hasOverlap)
            {
                return true;
            }
        }

        return false;
    }
}