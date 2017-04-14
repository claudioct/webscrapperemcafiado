using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.UI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Books.UI.Controllers
{
    public class BooksController : Controller
    {
        // GET: Books
        public JsonResult Index(string d, string autor, string nome)
        {
            if (string.IsNullOrWhiteSpace(autor) == false && string.IsNullOrWhiteSpace(nome) == false)
            {
                BooksDao booksDao = new BooksDao();
                var bookColletion = booksDao.GetBook(autor, nome);
                return Json(bookColletion);
            }
            else
            {
                BooksDao booksDao = new BooksDao();
                var bookColletion = booksDao.GetBooks(d);
                return Json(bookColletion);
            }
        }
    }
}