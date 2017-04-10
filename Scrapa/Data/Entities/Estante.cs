using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapa.Data.Entities
{
    public class Estante
    {
        [Key]
        public int IdEstante { get; set; }

        [StringLength(255)]
        [Index(IsUnique = true)]
        public string Nome { get; set; }

        public string Link { get; set; }
    }
}
