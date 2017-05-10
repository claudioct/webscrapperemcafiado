using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Books.UI.Data
{
    [BsonIgnoreExtraElements]
    public class BookFind
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Nome")]
        public string nome { get; set; }

        [BsonElement("Autor")]
        public string autor { get; set; }

        [BsonElement("QtdVendida")]
        public int? qtdVendida { get; set; }

        [BsonIgnore]
        public string info => $"{nome} - {autor}";
    }
}
