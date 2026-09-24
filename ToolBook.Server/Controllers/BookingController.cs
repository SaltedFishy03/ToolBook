using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Server.DTOs.Booking;
using ToolBook.Server.Enums;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;
using ToolBook.Server.Services.Results;

namespace ToolBook.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var id))
            {
                return null;
            }

            return id;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BookingResponse>>> GetAllBookings()
        {
            return Ok(await bookingService.GetAllAsync());
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<MyBookingResponse>>> MyBookings()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var bookings = await bookingService.GetByUserIdAsync(userId.Value);

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponse>> GetBookingById(int id)
        {
            var booking = await bookingService.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound("Bookingen med dette id findes ikke");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Admin") && userId != booking.UserId)
            {
                return Unauthorized();
            }

            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest bookingRequest)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await bookingService.CreateAsync(bookingRequest, userId.Value);

            switch (result.Error)
            {
                case BookingError.InvalidDate:
                    return BadRequest();

                case BookingError.ToolNotFound:
                    return NotFound("Værktøj ikke fundet");

                case BookingError.ToolUnavailable:
                    return Conflict("Værktøjet er ikke tilgængligt");

                case BookingError.Overlap:
                    return Conflict("Bookingen kolliderer med en allerde eksiteren booking");

                case BookingError.UserNotFound:
                    return Unauthorized();

                case BookingError.None:
                    if (result.Booking == null)
                    {
                        return NotFound("Bookingen blev oprettet, men resultatet mangler");
                    }

                    return CreatedAtAction(nameof(GetBookingById), new { id = result.Booking.Id }, result.Booking);

                default:
                    return Conflict("noget uventet gik galt");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponse>> UpdateBooking(int id, UpdateBookingRequest bookingRequest)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await bookingService.UpdateAsync(id, bookingRequest, userId.Value);

            switch (result.Error)
            {
                case BookingError.BookingNotFound:
                    return NotFound("Bookingen ikke fundet");

                case BookingError.BookingNotEditable:
                    return Conflict("Bookingen kan ikke længere ændres");

                case BookingError.InvalidDate:
                    return BadRequest();

                case BookingError.ToolNotFound:
                    return NotFound("Værktøj ikke fundet");

                case BookingError.ToolUnavailable:
                    return Conflict("Værktøjet er ikke tilgængligt");

                case BookingError.Overlap:
                    return Conflict("Bookingen kolliderer med en allerde eksiteren booking");

                case BookingError.None:
                    return Ok(result.Booking);

                default:
                    return Conflict("noget uventet gik galt");
            }
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await bookingService.CancelAsync(id, userId.Value);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("kunne ikke annulleres");
            }

            return NoContent();
        }

        [HttpPatch("{id}/return")]
        public async Task<IActionResult> ReturnBooking(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await bookingService.ReturnAsync(id, userId.Value);

            if (result == null)
            {
                return NotFound();
            }

            if (result == false)
            {
                return Conflict("Bookingen kan ikke registreres som afleveret");
            }

            return NoContent();
        }
        
    }
}