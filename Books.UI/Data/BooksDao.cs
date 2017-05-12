using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Books.UI.Helper;
using MySql.Data.MySqlClient;

namespace Books.UI.Data
{
    public class BooksDao
    {
        private string _connString;

        public BooksDao()
        {
            _connString =
                @"server=booksapp.mysql.dbaas.com.br;userid=booksapp;password=SenhaBolada123;database=booksapp";
            //_connString = @"Data Source=CLAUDIO-PC\Books;Initial Catalog=Books;Integrated Security=false;User=sa;Password=regina1;";
        }

        public object[] GetBooks(string search)
        {
            try { 
            var query = $@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT Nome, Autor 
FROM Livroes L
WHERE UPPER(L.Nome) LIKE UPPER('%{search}%')
	OR UPPER (L.Autor) LIKE UPPER('%{search}%')
ORDER BY L.QtdVendida DESC
LIMIT 5";
            var books = new List<object>();

            

            using (var connection = new MySqlConnection(_connString))
            {
                var command = new MySqlCommand(query, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new
                        {
                            nome = reader[0].ToString(),
                            autor = reader[1].ToString(),
                            info = $"{reader[0].ToString()} - {reader[1].ToString()}"
                        });
                    }
                }
            }
                return books.ToArray();
            }

                catch (Exception ex)
            {
                return new []{ new {error = ex.ToString()} };
            }
        }


        public object GetBook(string autor, string nome)
        {
            try
            {
                var query =
                    $@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT E.Nome Genero, L.Nome, L.Autor, L.QtdVendida, L.DtUltimaVenda, L.ValorUltimaVenda, L.ValorMaxVenda, L.ValorMinVenda, L.ValorMedioVenda 
FROM Livroes L 
	INNER JOIN Estantes E ON E.IdEstante = L.IdEstante
WHERE UPPER(L.Autor) = UPPER(@autor)
AND UPPER(L.Nome) = UPPER(@nome)";

                using (var connection = new MySqlConnection(_connString))
                {
                    var cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.Add("@autor", MySqlDbType.VarChar).Value = autor;
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = nome;
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var book = new
                            {
                                nome = reader.SafeGetString(reader.GetOrdinal("Nome")),
                                autor = reader.SafeGetString(reader.GetOrdinal("Autor")),
                                genero = reader.SafeGetString(reader.GetOrdinal("Genero")),
                                qtd = reader.SafeGetInt32(reader.GetOrdinal("QtdVendida")),
                                min = reader.SafeGetString(reader.GetOrdinal("ValorMinVenda")),
                                max = reader.SafeGetString(reader.GetOrdinal("ValorMaxVenda")),
                                media = reader.SafeGetString(reader.GetOrdinal("ValorMedioVenda")),
                                dtUltimaVenda = reader.SafeGetString(reader.GetOrdinal("DtUltimaVenda")),
                                vlUltimaVenda = reader.SafeGetString(reader.GetOrdinal("ValorUltimaVenda"))
                            };

                            return book;
                        }
                    }
                }
                return new { };
            }
            catch (Exception ex)
            {
                return new {error = ex.ToString()};
            }
        }

        public List<LivroEntity> GetAllBooks()
        {



            List<LivroEntity> books = new List<LivroEntity>();
            var query =
                $@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT  L.IdLivro, E.Nome Genero, L.Nome, L.Autor, L.QtdVendida, L.DtUltimaVenda, L.ValorUltimaVenda, L.ValorMaxVenda, L.ValorMinVenda, L.ValorMedioVenda 
FROM Livroes L INNER JOIN Estantes E ON E.IdEstante = L.IdEstante;";
#if DEBUG
            {
                query += @"
ORDER BY L.QtdVendida DESC
LIMIT 5";
            }
#endif


            using (var connection = new MySqlConnection(_connString))
            {
                var cmd = new MySqlCommand(query, connection);

                connection.Open();
                cmd.CommandTimeout = 180;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new LivroEntity()
                        {
                            id = reader.GetInt32(reader.GetOrdinal("IdLivro")),
                            nome = reader.SafeGetString(reader.GetOrdinal("Nome")),
                            autor = reader.SafeGetString(reader.GetOrdinal("Autor")),
                            genero = reader.SafeGetString(reader.GetOrdinal("Genero")),
                            qtd = reader.SafeGetInt32(reader.GetOrdinal("QtdVendida")),
                            min = reader.SafeGetString(reader.GetOrdinal("ValorMinVenda")),
                            max = reader.SafeGetString(reader.GetOrdinal("ValorMaxVenda")),
                            media = reader.SafeGetString(reader.GetOrdinal("ValorMedioVenda")),
                            dtUltimaVenda = reader.SafeGetString(reader.GetOrdinal("DtUltimaVenda")),
                            vlUltimaVenda = reader.SafeGetString(reader.GetOrdinal("ValorUltimaVenda"))
                        };

                        book.nomeNormalized = book.nome.RemoveDiacritics().ToUpper();
                        book.autorNormalized = book.autor.RemoveDiacritics().ToUpper();
                        book.search = $"{book.nomeNormalized} {book.autorNormalized}";

                        books.Add(book);
                    }
                }
            }
            return books;
        }
    }

    public static class DbUtil
    {
        public static string SafeGetString(this MySqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetString(index);
            return string.Empty;
        }

        public static DateTime? SafeGetDateTime(this MySqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return IntToDateTime(reader.GetInt32(index));
            return null;
        }

        private static DateTime? IntToDateTime(int dateAsInt)
        {
            return DateTime.ParseExact(dateAsInt.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static int? SafeGetInt32(this MySqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetInt32(index);
            return null;
        }

    }
}
