using Cinema.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class FilmeController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            return View();
        }


    }
}
