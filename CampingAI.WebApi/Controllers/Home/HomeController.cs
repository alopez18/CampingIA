using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.Home;
public class HomeController : Controller {
    public IActionResult Index() {
        return View();
    }
}