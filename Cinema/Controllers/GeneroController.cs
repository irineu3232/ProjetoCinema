using Cinema.Autenticao;
using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cinema.Controllers
{
    [SessionAuthorize(RoleAnyOf = "admin,gerente")]
    public class GeneroController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            var lista = new List<Genero>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("listar_genero", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Genero
                {
                    id_gen = rd.GetInt32("id_Gen"),
                    nomeGen = rd.GetString("nomeGen")
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
            using var cmd = new MySqlCommand("cad_genero", conn) { CommandType = CommandType.StoredProcedure };
            var generos = genero.nomeGen.ToLower();
            cmd.Parameters.AddWithValue("g_nome", generos);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id_gen)
        {
            Genero? genero = null;
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("buscar_genero", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("g_id", id_gen);
            var rd = cmd.ExecuteReader();
            while(rd.Read())
            {
                genero = new Genero
                {
                    id_gen = rd.GetInt32("id_Gen"),
                    nomeGen = rd.GetString("nomeGen")
                };
            }


            return View(genero);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(int id_gen, Genero genero)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_genero", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("g_nome", genero.nomeGen);
            cmd.Parameters.AddWithValue("g_id", id_gen);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("deletar_genero", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("g_id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }



    }
}
