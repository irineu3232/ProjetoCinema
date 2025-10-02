using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class FilmeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
