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
                        titulos = rd.GetString("titulo")
                    });
                }

            }

            using (var cmdAll = new MySqlCommand("buscar_premiacao", conn) { CommandType = CommandType.StoredProcedure})
            {
                cmdAll.Parameters.AddWithValue("p_q", "");
                cmdAll.Parameters.AddWithValue("c_t", "");
                using var rd2 = cmdAll.ExecuteReader();
                while(rd2.Read())
                {
                    var titulo = rd2.GetString("titulo");
                    if (!string.IsNullOrWhiteSpace(titulo) && !titulos.Contains(titulo)) ;
                    titulos.Add(titulo);
                }
            }

            ViewBag.q = q ?? "";
            ViewBag.Titulos = titulos;
            ViewBag.Genero = CarregarGenero(conn);

            return View(lista);


        }


        public IActionResult Detalhes(int id)
        {
            Filme filmes = null;
            List<(string Genero, string Diretor, string Premiacao)> premiacao = new();
            using (var conn = db.GetConnection())
            {
                string query = @"
                 select f.id_filme, f.titulo, f.genero 
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
                           genero = reader.GetInt32("genero")
                        };
                    }
                }

                // Buscar participações
                string participacaoQuery = @"
                        select  p.nomePremio, g.nomeGen, d.nome
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
                            reader.IsDBNull(reader.GetOrdinal("nomePremio"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nomePremio")),

                            reader.IsDBNull(reader.GetOrdinal("nomeGen"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nomeGen")),

                            reader.IsDBNull(reader.GetOrdinal("nome"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nome"))


                        ));
                    }

                }
            }

            ViewBag.Participacoes = premiacao;
            return View(filmes);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar( Premiacoes premio)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("cad_premiacao", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nomePremio", premio.premio);
            cmd.Parameters.AddWithValue("p_filme", premio.filme);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            Premiacoes? premiacao = null;
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("select id_premiacao, id_filme, nomePremiacao from Premiacoes where id_premiacao = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rd = cmd.ExecuteReader();

            if(rd.Read())
            {
                premiacao = new Premiacoes{

                    id_premiacoes = rd.GetInt32("id_premiacao"),
                    filme = rd.GetInt32("id_filme"),
                    premio = rd.GetString("nomePremiacao")
                } ;
            }


            ViewBag.Filme = CarregarFilme(conn);
            ViewBag.Diretor = CarregarDiretor(conn);

            return View( premiacao);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public  IActionResult Editar(int id, Premiacoes premio)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("editar_premiacao", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nomePremio", premio.premio);
            cmd.Parameters.AddWithValue("p_filme", premio.filme);
            cmd.Parameters.AddWithValue("p_id", premio.id_premiacoes);
            cmd.ExecuteNonQuery();

            return View();
        }

        [HttpPost]
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
