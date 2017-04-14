using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Books.UI.Data
{
    public class BooksDao
    {
        public BooksDao()
        {
                
        }

        public object[] GetBooks(string search)
        {
            try { 
            var query = $@"SELECT TOP 25 Nome, Autor 
FROM [dbo].[Livroes] AS [L]
WHERE UPPER ([L].[Nome] + ' ' + [L].[Autor]) LIKE UPPER('%{search}%')
	OR UPPER ([L].[Autor] + ' ' + [L].[Nome]) LIKE UPPER('%{search}%')
ORDER BY [L].[QtdVendida] DESC";
            var books = new List<object>();
            using (var connection = new SqlConnection(
                @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Scrapa.Data.ScrapaContext;Integrated Security=True;User ID=SomeUser;Password=topsecret")
            )
            {
                var command = new SqlCommand(query, connection);
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
                    $@"SELECT [E].[Nome] Genero, L.[Nome], [L].[Autor], [L].[QtdVendida], [L].[DtUltimaVenda], [L].[ValorUltimaVenda], [L].[ValorMaxVenda], [L].[ValorMinVenda], [L].[ValorMedioVenda] FROM [dbo].[Livroes] AS [L] INNER JOIN [dbo].[Estantes] AS [E] ON [E].[IdEstante] = [L].[IdEstante]
WHERE UPPER(L.[Autor]) = UPPER(@autor)
AND UPPER(L.[Nome]) = UPPER(@nome)";

                using (var connection = new SqlConnection(
                    @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Scrapa.Data.ScrapaContext;Integrated Security=True;User ID=SomeUser;Password=topsecret")
                )
                {
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.Add("@autor", SqlDbType.VarChar).Value = autor;
                    cmd.Parameters.Add("@nome", SqlDbType.VarChar).Value = nome;
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
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
    }

    public static class DbUtil
    {
        public static string SafeGetString(this SqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetString(index);
            return string.Empty;
        }

        public static DateTime? SafeGetDateTime(this SqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return IntToDateTime(reader.GetInt32(index));
            return null;
        }

        private static DateTime? IntToDateTime(int dateAsInt)
        {
            return DateTime.ParseExact(dateAsInt.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static int? SafeGetInt32(this SqlDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetInt32(index);
            return null;
        }

    }
}
