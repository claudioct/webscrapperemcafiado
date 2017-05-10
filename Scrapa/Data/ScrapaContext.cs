using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapa.Data.Entities;

namespace Scrapa.Data
{
    public class ScrapaContext : DbContext
    {
        public ScrapaContext() : base(@"Data Source=CLAUDIO-PC\books;Initial Catalog=booksApp;Integrated Security=True")
        {
                
        }

        public DbSet<Estante> Estantes { get; set; }

        public DbSet<Livro> Livros { get; set; }
    }
}
