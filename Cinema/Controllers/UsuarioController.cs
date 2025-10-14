using System.Data;
using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;

namespace Cinema.Controllers
{
    public class UsuarioController : Controller
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
        public IActionResult Criar(Usuario usuario)
        {

            var senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha, workFactor: 12);
            

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("cad_Usuario", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("u_nome", usuario.Nome);
            cmd.Parameters.AddWithValue("u_email", usuario.Email);
            cmd.Parameters.AddWithValue("u_senha", senha);
            cmd.Parameters.AddWithValue("u_role", usuario.role);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            Usuario usuario = null;

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("obter_usuario", conn) { CommandType= CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("u_id", id);
            using var rd = cmd.ExecuteReader();
            if(rd.Read())
            {
               usuario = new Usuario
               {
                   Id = rd.GetInt32(),
                   Nome.rd.GetString(),
               }
               ;
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult Editar(int id, Usuario usuario)
        {
            var senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha, workFactor : 12);
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_usuario",conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("u_nome", usuario.Nome);
            cmd.Parameters.AddWithValue("u_email", usuario.Email);
            cmd.Parameters.AddWithValue("u_senha", senha);
            cmd.Parameters.AddWithValue("u_role", usuario.role);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Excluir()
        { 
            return View(); 
        }



    }
}
