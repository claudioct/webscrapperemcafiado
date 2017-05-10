using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scrapa.Data.Entities;

namespace Scrapa.Data.MongoEntities
{
    public class Book
    {

        public Book(Livro livro)
        {
            this.Estante = livro.Estante.Nome;
            this.Nome = livro.Nome;
            this.Autor = livro.Autor;
            this.IdEstanteVirtual = livro.IdEstanteVirtual;
            this.QtdVendida = livro.QtdVendida;
            this.DtUltimaVenda = livro.DtUltimaVenda;
            this.ValorUltimaVenda = livro.ValorUltimaVenda;
            this.ValorMaxVenda = livro.ValorMaxVenda;
            this.ValorMinVenda = livro.ValorMinVenda;
            this.ValorMedioVenda = livro.ValorMedioVenda;
        }

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
