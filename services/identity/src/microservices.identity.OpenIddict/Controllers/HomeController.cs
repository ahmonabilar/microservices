using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace microservices.identity.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
