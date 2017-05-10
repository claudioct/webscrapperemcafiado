using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace Books.UI.Data
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; set; }

        public MongoContext()
        {
            var connString = "mongodb://localhost";
            var dbName = "booksDb";
            var client = new MongoClient(connString);
            Database = client.GetDatabase(dbName);
        }

        public IMongoCollection<Book> Books => Database.GetCollection<Book>("books");
    }
}
