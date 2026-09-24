using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.Tools;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Controllers
{
    public class ToolController(IHttpClientFactory httpClientFactory) : BaseController(httpClientFactory)
    {
        // GET: ToolController
        public async Task<IActionResult> Index(int? toolTypeId, int? categoryId, DateOnly? startDate, DateOnly? endDate)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();

            
            var categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? [];
            var toolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? [];

            // Valider datoerne
            ValidateDates(startDate, endDate);

            List<ToolTypeResponse> filteredToolTypes;

            if (ModelState.IsValid)
            {
                var filterUrl = "api/ToolType/filter";
                var queryParameters = new List<string>();

                if (toolTypeId.HasValue)
                {
                    queryParameters.Add($"toolTypeId={toolTypeId.Value}");
                }

                if (categoryId.HasValue)
                {
                    queryParameters.Add($"categoryId={categoryId.Value}");
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    queryParameters.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                    queryParameters.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                }

                if (queryParameters.Count > 0)
                {
                    filterUrl += "?" + string.Join("&", queryParameters);
                }

                filteredToolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>(filterUrl) ?? [];
            }
            else
            {
                // Ved ugyldigt filter viser vi siden igen uden at kalde API-filteret
                filteredToolTypes = toolTypes;
            }

            var model = new ToolOverviewViewModel
            {
                ToolTypes = toolTypes,
                FilteredToolTypes = filteredToolTypes,
                Categories = categories,

                ToolTypeId = toolTypeId,
                CategoryId = categoryId,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id, DateOnly? startDate, DateOnly? endDate)
        {
            var accessResult = RequireLogin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            
            // Valider datoerne
            ValidateDates(startDate, endDate);

            // Byg URL
            var detailsUrl = $"api/Tool/by-type/{id}";

            if (ModelState.IsValid && startDate.HasValue && endDate.HasValue)
            {
                detailsUrl += $"?startDate={startDate.Value:yyyy-MM-dd}&endDate={endDate.Value:yyyy-MM-dd}";
            }
            
            var tools = await client.GetFromJsonAsync<List<ToolDetailsResponse>>(detailsUrl) ?? [];
            var toolName = tools.FirstOrDefault()?.ToolTypeName ?? string.Empty;
            
            var model = new ToolDetailsViewModel
            {
                ToolTypeId = id,
                Tools = tools,
                ToolTypeName = toolName,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(model);
        }
        
        private void ValidateDates(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate.HasValue != endDate.HasValue)
            {
                ModelState.AddModelError("", "Vælg både startdato og slutdato.");
                return;
            }

            if (!startDate.HasValue)
            {
                return;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (startDate.Value < today)
            {
                ModelState.AddModelError("", "Startdato må ikke være før i dag.");
            }

            if (endDate!.Value < startDate.Value)
            {
                ModelState.AddModelError("", "Slutdato må ikke være før startdato.");
            }
        }
    }
}