using Microsoft.AspNetCore.Mvc;
using ToolBook.Client.Enums;
using ToolBook.Client.Models.Bookings;
using ToolBook.Client.Models.Tools;

namespace ToolBook.Client.Controllers
{
    public class BookingController(IHttpClientFactory httpClientFactory) : BaseController(httpClientFactory)
    {
        // GET: BookingController
        public async Task<IActionResult> Index()
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            
            
            var bookings = await client.GetFromJsonAsync<List<MyBookingResponse>>("api/Booking/my") ?? [];
            
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequest request, int toolTypeId)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();

            
            var response = await client.PostAsJsonAsync("api/Booking", request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "Bookingen kunne ikke oprettes. Værktøjet kan være blevet booket af en anden.";

                return RedirectToAction("Details", "Tool", new
                {
                    id = toolTypeId,
                    startDate = request.StartDate,
                    endDate = request.EndDate
                });
            }

            TempData["SuccessMessage"] = "Bookingen blev oprettet.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }

            var client = CreateAuthenticatedClient();

            var booking = await client.GetFromJsonAsync<MyBookingResponse>($"api/Booking/{id}");

            if (booking == null)
            {
                return RedirectToAction("Index");
            }

            var tools = await client.GetFromJsonAsync<List<ToolResponse>>("api/Tool") ?? [];

            var availableTools = tools
                .Where(t => t.Status == ToolStatus.Available || t.Id == booking.ToolId)
                .ToList();

            var model = new EditBookingViewModel
            {
                Id = booking.Id,
                ToolId = booking.ToolId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Tools = availableTools
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateBookingRequest request)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            var response = await client.PutAsJsonAsync($"api/Booking/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Bookingen kunne ikke ændres.";

                return RedirectToAction("Edit", new { id });
            }

            TempData["SuccessMessage"] = "Bookingen blev ændret.";

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Return(int id)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            var response = await client.PatchAsync($"api/Booking/{id}/return", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Værktøjet kunne ikke registreres som afleveret.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Værktøjet er registreret som afleveret.";

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            var response = await client.PatchAsync($"api/Booking/{id}/cancel", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Bookingen kunne ikke annulleres.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Bookingen blev annulleret.";

            return RedirectToAction("Index");
        }
    }
}
