using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace qms_pure.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
