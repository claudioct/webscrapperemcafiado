using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Livros.UI.Data;

namespace Livros.UI.Controllers
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
                return Json(bookColletion, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BooksDao booksDao = new BooksDao();
                var bookColletion = booksDao.GetBooks(d);
                return Json(bookColletion, JsonRequestBehavior.AllowGet);
            }
        }
    }
}