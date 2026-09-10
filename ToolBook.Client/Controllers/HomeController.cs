using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Client.Models;

namespace ToolBook.Client.Controllers;


public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Token") == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}