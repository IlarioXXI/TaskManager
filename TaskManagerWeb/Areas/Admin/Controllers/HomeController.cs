using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManagerWEB.Models;

namespace TaskManagerWEB.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {

        if (User.Identity.IsAuthenticated)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            return RedirectToAction("Index","Team");
        }
        return Redirect("identity/account/login");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
