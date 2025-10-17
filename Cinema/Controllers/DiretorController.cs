using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using Cinema.Autenticao;

namespace Cinema.Controllers
{
    [SessionAuthorize(RoleAnyOf = "admin,gerente")]
    public class DiretorController : Controller
    {

        private readonly Database db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Diretor>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("listar_diretor", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Diretor
                {
                    id_diretor = rd.GetInt32("id_diretor"),
                    nome = rd.GetString("nome"),
                    pais_origem = rd.GetString("pais_origem")
                });
            }
            return View(lista);

         
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Diretor diretor)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("cad_Diretor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("d_nome",diretor.nome);
            cmd.Parameters.AddWithValue("d_pais", diretor.pais_origem);
            cmd.ExecuteNonQuery();
            
            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            Diretor? diretor = null;
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("buscar_diretor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("d_id", id);
            var rd = cmd.ExecuteReader();
            
            while(rd.Read())
            {
                diretor = new Diretor
                {
                    id_diretor = rd.GetInt32("id_diretor"),
                    nome = rd.GetString("nome"),
                    pais_origem = rd.GetString("pais_origem")
                };
            }

            return View(diretor);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Diretor diretor)
        {

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_diretor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("id_di", diretor.id_diretor);
            cmd.Parameters.AddWithValue("d_nome", diretor.nome);
            cmd.Parameters.AddWithValue("d_pais", diretor.pais_origem);
            cmd.ExecuteNonQuery();
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("deletar_diretor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("d_id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }
    }
}
