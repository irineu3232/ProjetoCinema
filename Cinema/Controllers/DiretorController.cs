using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class DiretorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
