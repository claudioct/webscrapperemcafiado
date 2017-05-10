using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.UI.Data
{
    public class LivroEntity
    {
        public string nomeNormalized { get; set; }
        public string autorNormalized { get; set; }
        public string nome { get; set; }
        public string autor { get; set; }
        public string genero { get; set; }
        public int? qtd { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public string media { get; set; }
        public string dtUltimaVenda { get; set; }
        public string vlUltimaVenda { get; set; }

        public string info => $"{nome} - {autor}";
        public string search { get; set; }
        public int id { get; set; }
    }
}
