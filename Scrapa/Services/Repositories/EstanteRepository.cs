using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapa.Data;
using Scrapa.Data.Entities;

namespace Scrapa.Services.Repositories
{
    public class EstanteRepository
    {
        private readonly ScrapaContext _context;

        public EstanteRepository(ScrapaContext context)
        {
            _context = context;
        }

        public void Add(Estante estante)
        {
            _context.Estantes.Add(estante);
        }

        public void AddRange(IEnumerable<Estante> estanteCollection)
        {
            _context.Estantes.AddRange(estanteCollection);
        }

        public bool Save()
        {
            if (_context.SaveChanges() > 0)
                return true;
            else
                return false;
        }

        public bool Exists(string estanteNome)
        {
            return _context.Estantes.Any(e => e.Nome.ToUpper() == estanteNome.ToUpper());
        }

        public IEnumerable<Estante> GetAll()
        {
            return _context.Estantes.ToList();
        }
    }
}
