using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
