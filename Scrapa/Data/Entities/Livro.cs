using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapa.Data.Entities
{
    public class Livro
    {
        [Key]
        public long IdLivro { get; set; }

        [ForeignKey("Estante")]
        [Required]
        public int IdEstante { get; set; }

        public Estante Estante { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nome { get; set; }

        [Required]
        [StringLength(1000)]
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
