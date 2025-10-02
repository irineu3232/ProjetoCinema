using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;


namespace Cinema.Controllers
{
    public class PreamicaoController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            return View();
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

   
    

    
    
    }
}
