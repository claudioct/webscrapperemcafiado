using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Books.UI.Data;
using Books.UI.Helper;
using Books.UI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Books.UI.Controllers
{
    public class BooksController : Controller
    {
        private IMemoryCache _cache;
        private string cacheKey = "livrosCache";
        private string isRecuperandoKey = "isRecuperandoKey";

        public BooksController(IMemoryCache cache)
        {
            _cache = cache;
        }
        // GET: Books
        public JsonResult Index(string d, string autor, string nome)
        {
            //Isso está aqui pois estou com preguiça de atualizar o código do request no angular =)
            string busca = d;
            if (_cache.TryGetValue(cacheKey, out List<LivroEntity> livroCollectionCache))
            {
                if (string.IsNullOrWhiteSpace(autor) == false && string.IsNullOrWhiteSpace(nome) == false)
                {
                    nome = nome.ToUpper().RemoveDiacritics();
                    autor = autor.ToUpper().RemoveDiacritics();
                    var book =
                        livroCollectionCache.FirstOrDefault(
                            b => 
                            b.nomeNormalized.ToUpper() == nome 
                            && b.autorNormalized.ToUpper() == autor);
                    return Json(book);
                }
                else
                {
                    busca = busca.ToUpper();
                    //var books =
                    //    livroCollectionCache.Where(
                    //        b =>
                    //                b.nomeNormalized.Contains(busca)
                    //                || b.autorNormalized.Contains(busca)
                    //        ).OrderByDescending(l => l.qtd).Take(5);
                    var books =
                        livroCollectionCache.Where(
                            b =>
                                    b.search.Contains(busca.Split(' '))
                                    
                            ).OrderByDescending(l => l.qtd).Take(5);

                    return Json(books);
                }
            }
            else
            {
                BooksCache.Recuperar(_cache);

                if (string.IsNullOrWhiteSpace(autor) == false && string.IsNullOrWhiteSpace(nome) == false)
                {
                    BooksDao booksDao = new BooksDao();
                    var bookColletion = booksDao.GetBook(autor, nome);
                    return Json(bookColletion);
                }
                else
                {
                    BooksDao booksDao = new BooksDao();
                    var bookColletion = booksDao.GetBooks(busca);
                    return Json(bookColletion);
                }
            }
        }


    }
}