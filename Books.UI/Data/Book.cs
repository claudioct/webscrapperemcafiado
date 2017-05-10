using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Books.UI.Data
{
    public class Book
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Estante { get; set; }
        public string Nome { get; set; }
        public string Autor { get; set; }
        public long IdEstanteVirtual { get; set; }
        public int QtdVendida { get; set; }
        public string DtUltimaVenda { get; set; }
        public string ValorUltimaVenda { get; set; }
        public string ValorMaxVenda { get; set; }
        public string ValorMinVenda { get; set; }
        public string ValorMedioVenda { get; set; }
    }
}
