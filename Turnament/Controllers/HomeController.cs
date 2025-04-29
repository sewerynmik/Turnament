using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Turnament.Models;

namespace Turnament.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}