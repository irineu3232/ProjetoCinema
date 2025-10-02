using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class DiretorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar()

    }
}
