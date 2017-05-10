using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Scrapa.Data;
using Scrapa.Data.Entities;

namespace Scrapa.Services.Repositories
{
    class LivroRepository
    {
        private ScrapaContext _context;

        public LivroRepository(ScrapaContext scrapaContext)
        {
            this._context = scrapaContext;
        }

        public bool Exists(string bookName, string bookAuthor)
        {
            ScrapaContext scrapaContext = new ScrapaContext();
            var output = scrapaContext.Livros.Any(b => (string.Compare(bookName, b.Nome, StringComparison.InvariantCultureIgnoreCase) ==0) && (string.Compare(bookAuthor, b.Autor, StringComparison.InvariantCultureIgnoreCase) == 0));
            scrapaContext.Dispose();
            return output;

        }

        public List<Livro> GetAll()
        {
            return this._context.Livros
                .Include(x => x.Estante)
                .ToList();
        }

        public void Add(Livro livro)
        {
            _context.Livros.Add(livro);
        }

        public bool Save()
        {
            if (_context.SaveChanges() <= 0)
                return false;
            else
                return true;
        }

        internal void Add(ConcurrentBag<Livro> livros)
        {
            _context.Livros.AddRange(livros);
        }

        public void AddThreadSafe(ConcurrentBag<Livro> livros)
        {
            ScrapaContext scrapaContext = new ScrapaContext();
            scrapaContext.Livros.AddRange(livros);
            scrapaContext.SaveChanges();
            scrapaContext.Dispose();
        }
    }
}