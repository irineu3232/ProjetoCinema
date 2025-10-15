using Cinema.Autenticao;
using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;

namespace Cinema.Controllers
{
    public class AuthController : Controller
    {
        private readonly Database db = new Database();

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string senha, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Error = "Informe e-mail e senha";
                return View();
            }

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("buscar_usuario_login") { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("u_email", email);

            using var rd = cmd.ExecuteReader();

            if (!rd.Read())
            {
                ViewBag.Error = "Usuario não encontrado";
                return View();
            }

            var id = rd.GetInt32("id");
            var nome = rd.GetString("Nome");
            var role = rd.GetString("role");
            var ativo = rd.GetBoolean("Ativo");
            var senhaHash = rd["Senha"] as string;

            if (!ativo)
            {
                ViewBag.Error = "Usuario inátivo";
                return View();
            }

            bool ok;
            try
            {
                ok = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
            }
            catch
            {
                ok = false;
            }

            if (!ok)
            {
                ViewBag.Error = "Senha inválida";
                return View();
            }

            // ===== Setar sessão =====
            HttpContext.Session.SetInt32(SessionKey.UserId, id);
            HttpContext.Session.SetString(SessionKey.UserName, nome);
            HttpContext.Session.SetString(SessionKey.UserEmail, email);
            HttpContext.Session.SetString(SessionKey.UserRole, role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AcessoNegado() => View();



    }
}
