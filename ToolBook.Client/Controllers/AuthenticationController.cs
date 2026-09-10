using Microsoft.AspNetCore.Mvc;
using ToolBook.Client.Models.Auth;

namespace ToolBook.Client.Controllers
{
    public class AuthenticationController(IHttpClientFactory httpClientFactory) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validerer formularen ud fra DataAnnotations i ViewModel.
            // Hvis input er ugyldigt, vises samme view igen med fejlbeskeder.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Henter den konfigurerede HttpClient og sender login-data som JSON til API'et.
            var toolBookApi = httpClientFactory.CreateClient("ToolBookApi");
            var response = await toolBookApi.PostAsJsonAsync("api/auth/login", model);

            // Hvis API'et afviser requesten, læses fejlbeskeden
            // og vises som en generel fejl i formularen.
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("", errorMessage);

                return View(model);
            }

            // Konverterer JSON-svaret fra API'et til et AuthResponse-objekt.
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse == null)
            {
                ModelState.AddModelError("", "Der opstod en fejl i kommunikationen med serveren");
                return View(model);
            }

            // Gemmer JWT og brugeroplysninger i sessionen,
            // så clienten kan huske den loggede bruger mellem requests.
            HttpContext.Session.SetString("Token", authResponse.Token);
            HttpContext.Session.SetString("Name", authResponse.Name);
            HttpContext.Session.SetString("Role", authResponse.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validerer formularen ud fra DataAnnotations i ViewModel.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Mapper ViewModel til RegisterRequest.
            // ConfirmPassword bruges kun til client-validering og sendes derfor ikke til API'et.
            var request = new RegisterRequest
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            // Henter den konfigurerede HttpClient og sender registreringsdata som JSON til API'et.
            var toolBookApi = httpClientFactory.CreateClient("ToolBookApi");
            var response = await toolBookApi.PostAsJsonAsync("api/auth/register", request);

            // Hvis API'et afviser requesten, vises fejlbeskeden i formularen.
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("", errorMessage);

                return View(model);
            }

            // Konverterer JSON-svaret fra API'et til et AuthResponse-objekt.
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse == null)
            {
                ModelState.AddModelError("", "Der opstod en fejl i kommunikationen med serveren");
                return View(model);
            }

            // Gemmer JWT og brugeroplysninger i sessionen,
            // så brugeren er logget ind efter registreringen.
            HttpContext.Session.SetString("Token", authResponse.Token);
            HttpContext.Session.SetString("Name", authResponse.Name);
            HttpContext.Session.SetString("Role", authResponse.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Authentication");
        }
    }
}