using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cinema.Controllers
{
    public class GeneroController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            var lista = new List<Genero>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Genero
                {
                    id_gen = rd.GetInt32(""),
                    nomeGen = rd.GetString("")
                });
            }
            return View(lista);

        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Genero genero)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("",);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("",);
            cmd.ExecuteReader();

            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Genero genero)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("",);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("",);
            cmd.ExecuteNonQuery();

            return View();
        }



    }
}
