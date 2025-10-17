using Cinema.Autenticao;
using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Data;


namespace Cinema.Controllers
{
    public class PreamicaoController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index(string? q, string? g)
        {
            // Criar um listar filmes que tenhão premiações.
            // E quando apertar, ser jogado na área de detalhes!
            var lista = new List<Filme>();
            var titulos = new List<string>();
            var genero = new List<string>();
            using var conn = db.GetConnection();
            
            using var cmd = new MySqlCommand("buscar_premiacao", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            {
                cmd.Parameters.AddWithValue("p_q", q ?? "");
                cmd.Parameters.AddWithValue("c_t", g ?? "");
                using var rd = cmd.ExecuteReader();
                while(rd.Read())
                {
                    lista.Add(new Filme
                    {
                        id_filme = rd.GetInt32("id_filme"),
                        titulos = rd.GetString("titulo"),
                        genero = rd.GetInt32("genero"),
                        capa = rd.GetString("capa")
                    });
                }

            }
            using var conn2 = db.GetConnection();
            using (var cmdAll = new MySqlCommand("buscar_premiacao", conn2) { CommandType = CommandType.StoredProcedure})
            {
                cmdAll.Parameters.AddWithValue("p_q", "");
                cmdAll.Parameters.AddWithValue("c_t", "");
                using var rd2 = cmdAll.ExecuteReader();
                while(rd2.Read())
                {
                    var titulo = rd2.GetString("titulo");
                    if (!string.IsNullOrWhiteSpace(titulo) && !titulos.Contains(titulo)) 
                    titulos.Add(titulo);
                }
            }

            using var conn3 = db.GetConnection();

            ViewBag.q = q ?? "";
            ViewBag.g = g ?? "";
            ViewBag.Titulos = titulos;
            ViewBag.Genero = CarregarGenero(conn3);

            return View(lista);


        }


        public IActionResult Detalhes(int id)
        {
            Filme filmes = null;
            List<(string Genero, string Diretor, string Premiacao, string codpremiacao)> premiacao = new();
            using (var conn = db.GetConnection())
            {
                string query = @"
                 select f.id_filme, f.titulo, f.genero, f.id_diretor, f.capa 
                 from Filmes f
                 where id_filme = @id; 
                 ";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        filmes = new Filme
                        {
                           id_filme = reader.GetInt32("id_filme"),
                           id_diretor = reader.GetInt32("id_diretor"),
                           titulos = reader.GetString("titulo"),
                           genero = reader.GetInt32("genero"),
                           capa = reader.GetString("capa")
                        };
                    }
                }

                // Buscar participações
                string participacaoQuery = @"
                        select  p.nomePremio, g.nomeGen, d.nome, p.id_premiacao
                        from Premiacoes p 
                        inner join Filmes f on p.id_filme = f.id_filme
                        inner join Filmes_Genero g on f.genero = g.id_Gen
                        inner join Diretores d on  f.id_diretor = d.id_diretor
                        where f.id_filme = @id;
                        ";

                var cmd2 = new MySqlCommand(participacaoQuery, conn);
                cmd2.Parameters.AddWithValue("@id", id);
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        premiacao.Add((

                        reader.IsDBNull(reader.GetOrdinal("nomeGen")) ? null : reader.GetString(reader.GetOrdinal("nomeGen")),        // Genero (Item1)
                        reader.IsDBNull(reader.GetOrdinal("nome")) ? null : reader.GetString(reader.GetOrdinal("nome")),              // Diretor (Item2)
                        reader.IsDBNull(reader.GetOrdinal("nomePremio")) ? null : reader.GetString(reader.GetOrdinal("nomePremio")), // Premiação (Item3)
                        reader.IsDBNull(reader.GetOrdinal("id_premiacao")) ? "?" : reader.GetInt32(reader.GetOrdinal("id_premiacao")).ToString() // codpremiacao (Item4)

                        ));
                    }

                }
            }




            ViewBag.Participacoes = premiacao;
            return View(filmes);
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "admin,gerente")]
        public IActionResult Criar()
        {
            using var conn = db.GetConnection();
            ViewBag.Filme = CarregarFilme(conn);
            return View();
        }

        [HttpPost]
        public IActionResult Criar( Premiacoes premiacao)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("cad_premiacao", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nomePremio", premiacao.premio);
            cmd.Parameters.AddWithValue("p_filme", premiacao.filme);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "admin,gerente")]
        public IActionResult Editar(int id_premiacao)
        {
            Premiacoes? premiacao = null;
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("select id_premiacao, id_filme, nomePremio from Premiacoes where id_premiacao = @id", conn);
            cmd.Parameters.AddWithValue("@id", id_premiacao);
            var rd = cmd.ExecuteReader();

            if(rd.Read())
            {
                premiacao = new Premiacoes{

                    id_premiacoes = rd.GetInt32("id_premiacao"),
                    filme = rd.GetInt32("id_filme"),
                    premio = rd.GetString("nomePremio")
                } ;
            }

            using var conn2 = db.GetConnection();
            ViewBag.Filme = CarregarFilme(conn2);

            return View( premiacao);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public  IActionResult Editar(int id_premiacao, Premiacoes premiacao)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_premiacao", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nomePremio", premiacao.premio);
            cmd.Parameters.AddWithValue("p_filme", premiacao.filme);
            cmd.Parameters.AddWithValue("p_id", id_premiacao);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "admin,gerente")]
        public IActionResult Excluir(int id)
        {
            try
            {

                using var conn = db.GetConnection();
                using var cmd = new MySqlCommand("deletar_premiacao", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("p_id", id);
                cmd.ExecuteNonQuery();

                TempData["ok"] = "Premiação excluida";
            }
            catch(MySqlException ex)
            {
                TempData["ok"] = ex.Message;
            }
            return View();
        }



        private List<SelectListItem> CarregarDiretor(MySqlConnection conn)
        {
            var list = new List<SelectListItem>();
            using var cmd = new MySqlCommand("select id_diretor, nome from Diretores", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new SelectListItem { Value = rd.GetInt32("id_diretor").ToString(), Text = rd.GetString("nomeGen") });
            return list;
        }


        private List<SelectListItem> CarregarFilme(MySqlConnection conn)
        {
            var list = new List<SelectListItem>();
            using var cmd = new MySqlCommand("select id_filme, titulo from Filmes", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new SelectListItem { Value = rd.GetInt32("id_filme").ToString(), Text = rd.GetString("titulo") });
            return list;
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
