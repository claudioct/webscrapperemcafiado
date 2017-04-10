using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Gekoproject;
using HtmlAgilityPack;
using Scrapa.Data;
using Scrapa.Data.Entities;
using Scrapa.Helpers;
using Scrapa.Services.Repositories;

namespace Scrapa
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDatabase();
            ScrapaContext scrapaContext = new ScrapaContext();
            var estanteRepository = new EstanteRepository(scrapaContext);



            //ScrapTipoEstantes(estanteRepository);
            var baseUrl = "https://www.estantevirtual.com.br/";
            IEnumerable<Estante> estantes =  estanteRepository.GetAll();

            foreach (var estante in estantes)
            {
                var page = 1;

                while (true)
                {
                    try
                    {
                        WebClient GodLikeClient = new WebClient();
                        HtmlAgilityPack.HtmlDocument godLikeHTML = new HtmlAgilityPack.HtmlDocument();

                        HttpDownloader downloader = new HttpDownloader($"{estante.Link}&offset=1", null, null);
                        godLikeHTML.LoadHtml(downloader.GetPage());
                        
                        HtmlWeb web = new HtmlWeb();
                        //HtmlDocument findBooksDocument = web.Load($"{estante.Link}&offset=1");
                        var books = godLikeHTML.DocumentNode.SelectNodes("//*[contains(@class, 'js-busca-resultados')]/a");
                        
                        foreach (var book in books)
                        {
                            var bookName = book
                                .Descendants("span")
                                .Where(d =>
                                    d.Attributes.Contains("class")
                                    &&
                                    d.Attributes["class"].Value.Contains("busca-title"))
                                .FirstOrDefault().InnerText;

                            var bookAuthor = book
                                .Descendants("span")
                                .Where(d =>
                                    d.Attributes.Contains("class")
                                    &&
                                    d.Attributes["class"].Value.Contains("busca-author"))
                                .FirstOrDefault().InnerText;


                            var url = book.Attributes["href"];
                            Thread.Sleep(500);
                        }

                        page++;
                    }

                    catch (Exception e)
                    {
                        continue;
                    }
                }

            }
        }

        private static void ScrapTipoEstantes(EstanteRepository estanteRepository)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.estantevirtual.com.br/estantes");
            string[] tiposEstantes = new string[] { "menu-1", "menu-2", "menu-3", "menu-4" };
            foreach (var tipoEstante in tiposEstantes)
            {
                var estantes =
                    document.DocumentNode.SelectNodes(
                        $"//*[@id=\"{tipoEstante}\"]//*[contains(@class, 'list')]/li"
                    );
                List<Estante> estanteCollection = new List<Estante>();
                foreach (var item in estantes)
                {
                    Estante estante = new Estante();
                    estante.Nome = item.SelectSingleNode("a").InnerText;
                    // Para evitar duplicidade, se já houver no banco, pula para o próximo.
                    if (estanteRepository.Exists(estante.Nome)) { continue; }

                    estante.Link = item.SelectSingleNode("a").Attributes["href"].Value;
                    estanteCollection.Add(estante);
                }
                if (estanteCollection.Count > 0)
                {
                    estanteRepository.AddRange(estanteCollection);
                    estanteRepository.Save();
                }
            }
        }

        private static void CreateDatabase()
        {
            
        }
    }
}
