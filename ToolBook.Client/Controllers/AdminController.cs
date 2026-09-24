using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Client.Models.Admin;
using ToolBook.Client.Models.ToolCategories;
using ToolBook.Client.Models.Tools;
using ToolBook.Client.Models.ToolTypes;

namespace ToolBook.Client.Controllers
{
    public class AdminController(IHttpClientFactory httpClientFactory) : BaseController(httpClientFactory)
    {
        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            
            
            var tools = await client.GetFromJsonAsync<List<ToolResponse>>("api/Tool") ?? new();
            var toolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();
            var categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

            var model = new AdminViewModel
            {
                Tools = tools,
                ToolTypes = toolTypes,
                Categories = categories
            };

            return View(model);
        }
        
        [HttpGet]
        public IActionResult CreateCategory()
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateToolCategoryRequest request)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var client = CreateAuthenticatedClient();

            var response = await client.PostAsJsonAsync("api/ToolCategory", request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Kunne ikke oprette kategorien.");
                return View(request);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateToolType()
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();

            
            var categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

            var model = new CreateToolTypeViewModel
            {
                Categories = categories
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateToolType(CreateToolTypeViewModel model)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }

            var client = CreateAuthenticatedClient();

            if (!ModelState.IsValid)
            {
                model.Categories =
                    await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

                return View(model);
            }

            var response = await client.PostAsJsonAsync("api/ToolType", model.ToolType);

            if (!response.IsSuccessStatusCode)
            {
                model.Categories =
                    await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

                ModelState.AddModelError(
                    string.Empty,
                    "Kunne ikke oprette værktøjstypen.");

                return View(model);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateTool()
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            var toolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

            var model = new CreateToolViewModel
            {
                ToolTypes = toolTypes
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTool(CreateToolViewModel model)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            if (!ModelState.IsValid)
            {
                model.ToolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

                return View(model);
            }

            var response = await client.PostAsJsonAsync("api/Tool", model.Tool);

            if (!response.IsSuccessStatusCode)
            {
                model.ToolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

                ModelState.AddModelError(string.Empty, "Kunne ikke oprette værktøjet.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
        
        
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            
            
            var category = await client.GetFromJsonAsync<ToolCategoryResponse>($"api/ToolCategory/{id}");

            if (category == null)
            {
                return NotFound();
            }

            var model = new UpdateToolCategoryRequest
            {
                Name = category.Name,
                Description = category.Description
            };

            ViewBag.CategoryId = id;

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, UpdateToolCategoryRequest request)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(request);
            }
            var client = CreateAuthenticatedClient();
            

            var response = await client.PutAsJsonAsync($"api/ToolCategory/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.CategoryId = id;
                ModelState.AddModelError(string.Empty, "Kunne ikke opdatere kategorien.");

                return View(request);
            }

            return RedirectToAction("Index");
        }
        
        
        [HttpGet]
        public async Task<IActionResult> EditToolType(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            var toolType = await client.GetFromJsonAsync<ToolTypeResponse>($"api/ToolType/{id}");

            if (toolType == null)
            {
                return NotFound();
            }

            var categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

            var model = new EditToolTypeViewModel
            {
                ToolType = new UpdateToolTypeRequest
                {
                    Name = toolType.Name,
                    Description = toolType.Description,
                    CategoryId = toolType.CategoryId
                },
                Categories = categories
            };

            ViewBag.ToolTypeId = id;

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditToolType(int id, EditToolTypeViewModel model)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();
            

            if (!ModelState.IsValid)
            {
                model.Categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

                ViewBag.ToolTypeId = id;

                return View(model);
            }

            var response = await client.PutAsJsonAsync($"api/ToolType/{id}", model.ToolType);

            if (!response.IsSuccessStatusCode)
            {
                model.Categories = await client.GetFromJsonAsync<List<ToolCategoryResponse>>("api/ToolCategory") ?? new();

                ViewBag.ToolTypeId = id;

                ModelState.AddModelError(string.Empty, "Kunne ikke opdatere værktøjstypen.");

                return View(model);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> EditTool(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();


            var tool = await client.GetFromJsonAsync<ToolResponse>($"api/Tool/{id}");

            if (tool == null)
            {
                return NotFound();
            }

            var toolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

            var model = new EditToolViewModel
            {
                Tool = new UpdateToolRequest
                {
                    ToolNumber = tool.ToolNumber,
                    Status = tool.Status,
                    ToolTypeId = tool.ToolTypeId
                },

                ToolTypes = toolTypes
            };

            ViewBag.ToolId = id;

            return View(model);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> EditTool(int id, EditToolViewModel model)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();


            if (!ModelState.IsValid)
            {
                model.ToolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

                ViewBag.ToolId = id;

                return View(model);
            }

            var response = await client.PutAsJsonAsync($"api/Tool/{id}", model.Tool);

            if (!response.IsSuccessStatusCode)
            {
                model.ToolTypes = await client.GetFromJsonAsync<List<ToolTypeResponse>>("api/ToolType") ?? new();

                ViewBag.ToolId = id;

                ModelState.AddModelError(string.Empty, "Kunne ikke opdatere værktøjet.");

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();


            var response = await client.DeleteAsync($"api/ToolCategory/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kategorien kunne ikke slettes. Den kan være i brug af en værktøjstype.";
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteToolType(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();


            var response = await client.DeleteAsync($"api/ToolType/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Værktøjstypen kunne ikke slettes. Der kan være værktøjer tilknyttet den.";
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteTool(int id)
        {
            var accessResult = RequireAdmin();

            if (accessResult != null)
            {
                return accessResult;
            }
            var client = CreateAuthenticatedClient();


            var response = await client.DeleteAsync($"api/Tool/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Værktøjet kan ikke slettes, da det har bookinghistorik. Sæt værktøjets status til \"OutOfService\", hvis det ikke længere skal kunne bruges.";
            }

            return RedirectToAction("Index");
        }
    }
}
