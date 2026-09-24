using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ToolBook.Client.Controllers;

public abstract class BaseController(IHttpClientFactory httpClientFactory) : Controller
{
    protected IActionResult? RequireLogin()
    {
        if (HttpContext.Session.GetString("Token") == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        return null;
    }

    protected IActionResult? RequireAdmin()
    {
        var loginResult = RequireLogin();

        if (loginResult != null)
        {
            return loginResult;
        }

        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Tool");
        }

        return null;
    }

    protected HttpClient CreateAuthenticatedClient()
    {
        var token = HttpContext.Session.GetString("Token")
                    ?? throw new InvalidOperationException(
                        "Brugeren er ikke logget ind.");

        var client = httpClientFactory.CreateClient("ToolBookApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}