using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class UsuarioController : Controller
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
        {
            return View();
        }

        [HttpGet]
        public IActionResult Editar()
        {
            return View();
        }

        [HttpPost]
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
