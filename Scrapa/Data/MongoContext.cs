using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapa.Data.MongoEntities;

namespace Scrapa.Data
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
