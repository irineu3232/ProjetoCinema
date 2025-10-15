using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ZstdSharp.Unsafe;

namespace Cinema.Controllers
{
    public class FilmeController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            var lista = new List<Filme>();
            var listaDiretor = new List<Diretor>();
            var listaGenero = new List<Genero>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("listar_Filme", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Filme
                {
                    id_diretor = rd.GetInt32("f.id_diretor"),
                    titulos = rd.GetString("f.titulo"),
                    id_filme = rd.GetInt32("f.id_filme"),
                    genero = rd.GetInt32("f.genero")
                });
            }

            using var cmd2 = new MySqlCommand("select id_diretor, nome from Diretores", conn);
            var rd2 = cmd2.ExecuteReader();
            while(rd2.Read())
            {
                listaDiretor.Add(new Diretor
                {
                    id_diretor = rd.GetInt32("id_diretor"),
                    nome = rd.GetString("nomeDiretor")
                });

            }

            using var cmd3 = new MySqlCommand("Select id_gen, nomeGen from Filmes_Genero", conn);
            var rd3 = cmd3.ExecuteReader();
            while(rd3.Read())
            {
                listaGenero.Add(new Genero
                {
                    id_gen = rd.GetInt32("id_gen"),
                    nomeGen = rd.GetString("nomeGen")
                });
            }



            ViewBag.Diretor = listaDiretor;
            ViewBag.Genero = listaGenero;
            return View(lista);
            // Aqui para pegar quem vai estar relacionado a qual item...
            // Podemos usar e comparar o id_gen com o genero do filme... ex: genero == id_gen
            // E a mesma coisa para diretor id_diretor == id_diretor
            // Exemplo do chat, ele cria a variavel e faz a comparação, usando as viewbags, pegando a primeira ou valor default dos resultados na viewBag que correspondem ao valores em filmes
            //  var diretor = diretores.FirstOrDefault(d => d.IdDiretor == filme.IdDiretor);
            //  var genero = generos.FirstOrDefault(g => g.IdGen == filme.IdGenero);
        }

        public IActionResult Criar()
        {
            using var conn = db.GetConnection();
            ViewBag.Genero = CarregarGenero(conn);
            return View();
        }


        [HttpPost]
        public IActionResult Criar(Filme filme)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("cad_filme", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("f_titulo", filme.titulos);
            cmd.Parameters.AddWithValue("f_genero", filme.genero);
            cmd.Parameters.AddWithValue("f_diretor", filme.id_diretor);
            cmd.ExecuteNonQuery();

            return View();
        }


        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("obter_filme", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("f_id", id);
            cmd.ExecuteReader();

            ViewBag.Genero = CarregarGenero(conn);
            return View();
        }


        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Filme filme)
        {

            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_filme", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("f_titulo", filme.titulos);
            cmd.Parameters.AddWithValue("f_genero", filme.genero);
            cmd.Parameters.AddWithValue("f_diretor", filme.id_diretor);
            cmd.Parameters.AddWithValue("f_cod", filme.id_filme);
            cmd.ExecuteNonQuery();

            return View();
        }

        public IActionResult Excluir(int id)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("deletar_filme", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("f_id", id);
            cmd.ExecuteNonQuery();

            return View();
        }


        private List<SelectListItem> CarregarGenero(MySqlConnection conn)
        {
            var list = new List<SelectListItem>();
            using var cmd = new MySqlCommand("select id_gen, nomeGen from Filmes_Genero", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new SelectListItem { Value = rd.GetInt32("id_gen").ToString(), Text = rd.GetString("nomeGen") });
            return list;
        }

    }
}
