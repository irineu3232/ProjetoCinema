using Cinema.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class DiretorController : Controller
    {

        private readonly Database db = new Database();

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
        {
            return View();
        }

        [HttpGet]
        public IActionResult Editar()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Editar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Excluir()
        {
            return View();
        }
    }
}
