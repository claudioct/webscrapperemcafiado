using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json;
using Scrapa.Data;
using Scrapa.Data.Entities;
using Scrapa.Data.MongoEntities;
using Scrapa.Helpers;
using Scrapa.Services;
using Scrapa.Services.Repositories;

namespace Scrapa
{
    class Program
    {
        private static ILog _logger;
        static void Main(string[] args)
        {
            CreateDatabase();
            //RetrieveDataEstante();
            MigrateToMongo();
        }

        private static void MigrateToMongo()
        {
            var ctx = new Data.MongoContext();
            ScrapaContext scrapaContext = new ScrapaContext();
            LivroRepository livroRepository = new LivroRepository(scrapaContext);
            var livrosCollection = livroRepository.GetAll();
            var mongoBookCollection = new List<Book>();
            foreach (var livro in livrosCollection)
            {
                var book = new Book(livro);
                mongoBookCollection.Add(book);
            }

            ctx.Books.InsertMany(mongoBookCollection);
        }

        private static void RetrieveDataEstante()
        {
            
            Logger.Setup();
            _logger = LogManager.GetLogger(typeof(Program));

            ScrapaContext scrapaContext = new ScrapaContext();
            EstanteRepository estanteRepository = new EstanteRepository(scrapaContext);
            LivroRepository livroRepository = new LivroRepository(scrapaContext);


            //ScrapTipoEstantes(estanteRepository);
            string baseUrl = "https://www.estantevirtual.com.br";
            IEnumerable<Estante> estantes = estanteRepository.GetAll().Where(e => e.IdEstante > 38).ToList();

            foreach (Estante estante in estantes)
            {
                Console.WriteLine($@"Starting with estante {estante.Nome}");
                _logger.Info($"Starting with Estante {estante.Nome}");
                try
                {
                    CancellationTokenSource producerCancellationToken = new CancellationTokenSource();
                    CancellationTokenSource consumerCancellationToken = new CancellationTokenSource();

                    BlockingCollection<HtmlNode> pageCollection = new BlockingCollection<HtmlNode>(250);
                    Task producerThread = Task.Factory.StartNew(() =>
                    {

                        int i = -1;
                        Parallel.For(0, 350,
                            new ParallelOptions() { MaxDegreeOfParallelism = 3, CancellationToken = producerCancellationToken.Token },
                            (iPage) =>
                            {
                                if (producerCancellationToken.Token.IsCancellationRequested)
                                {
                                    pageCollection.CompleteAdding();
                                }

                                Interlocked.Increment(ref i);
                                //for (int iPage = 0; iPage < 250; ++iPage)
                                Console.WriteLine($"Retriving {i}.");
                                _logger.Info($"Retriving {i}.");
                                HtmlAgilityPack.HtmlDocument getPage = new HtmlAgilityPack.HtmlDocument();
                                HttpDownloader htmlDownloader =
                                    new HttpDownloader($"{estante.Link}&offset={i}", null, null);
                                getPage.LoadHtml(htmlDownloader.GetPage());

                                pageCollection.Add(getPage.DocumentNode);

                                int sleepTime = new Random(DateTime.Now.Millisecond).Next(5, 50);
                                Console.WriteLine(
                                    $@"Sleeping for {sleepTime}. Page {i}. is ready. OnThread: {
                                            Thread.CurrentThread.ManagedThreadId
                                        })");
                                _logger.Info(
                                    $"Sleeping for {sleepTime}. Page {i}. is ready (OnThread: {Thread.CurrentThread.ManagedThreadId})");
                            });

                        pageCollection.CompleteAdding();
                    });

                    Task consumerThread = Task.Factory.StartNew(() =>
                    {
                        long iPage = 0;
                        long errorCount = 0;

                        Parallel.For(0, 350, new ParallelOptions() { MaxDegreeOfParallelism = 3, CancellationToken = consumerCancellationToken.Token }, (i) =>
                        {
                            consumerCancellationToken.Token.ThrowIfCancellationRequested();

                            Interlocked.Increment(ref iPage);

                            try
                            {
                                if (pageCollection.TryTake(out HtmlNode item, TimeSpan.FromSeconds(30)) == false)
                                {
                                    if (producerCancellationToken.Token.IsCancellationRequested && pageCollection.Count == 0)
                                    {
                                        consumerCancellationToken.Cancel();
                                        return;
                                    }
                                };

                                HtmlNodeCollection books =
                                    item.SelectNodes("//*[contains(@class, 'js-busca-resultados')]/a");
                                ConcurrentBag<HtmlNode> booksConcurrentBag =
                                    new ConcurrentBag<HtmlNode>(books.ToList());

                                ConcurrentBag<Livro> livros = new ConcurrentBag<Livro>();

                                Parallel.ForEach(
                                    booksConcurrentBag,
                                    new ParallelOptions { MaxDegreeOfParallelism = 20 },
                                    book =>
                                    {
                                        HtmlNode header =
                                            book.ChildNodes.FirstOrDefault(
                                                d => d.Attributes.Contains("class") &&
                                                     d.Attributes["class"].Value.Contains("breslt-header"));

                                        string bookName = header
                                            .ChildNodes
                                            .FirstOrDefault(d => d.Attributes.Contains("class")
                                                                 &&
                                                                 d.Attributes["class"]
                                                                     .Value.Contains("busca-title"))
                                            .InnerText;

                                        string bookAuthor = header
                                            .ChildNodes
                                            .FirstOrDefault(d => d.Attributes.Contains("class")
                                                                 &&
                                                                 d.Attributes["class"]
                                                                     .Value.Contains("busca-author"))
                                            .InnerText;

                                        if (livroRepository.Exists(bookName, bookAuthor))
                                        {
                                            return;
                                        }

                                        if (livros.Any(b =>
                                            (string.Compare(bookName, b.Nome,
                                                 StringComparison.InvariantCultureIgnoreCase) == 0)
                                            &&
                                            (string.Compare(bookAuthor, b.Autor,
                                                 StringComparison.InvariantCultureIgnoreCase) == 0)))
                                        {
                                            return;
                                        }

                                        var url = book.Attributes["href"];
                                        Uri uri = new Uri(baseUrl + url.Value);

                                        Livro livro = new Livro();
                                        livro.IdEstante = estante.IdEstante;
                                        livro.Autor = bookAuthor;
                                        livro.Nome = bookName;
                                        livro.IdEstanteVirtual = long.Parse(uri.Segments[4]);

                                        PriceStatistics pricestats =
                                            new PriceStatisticsService().Get(livro.IdEstanteVirtual);
                                        if (pricestats != null)
                                        {
                                            livro.QtdVendida = pricestats.QtdVendida;
                                            livro.DtUltimaVenda = pricestats.DtUltimaVenda;
                                            livro.ValorMaxVenda = pricestats.ValorMaxVenda;
                                            livro.ValorMedioVenda = pricestats.ValorMedioVenda;
                                            livro.ValorMinVenda = pricestats.ValorMinVenda;
                                            livro.ValorUltimaVenda = pricestats.ValorUltimaVenda;
                                        }

                                        livros.Add(livro);
                                    });
                                livroRepository.AddThreadSafe(livros);

                                int sleep = new Random(DateTime.Now.Millisecond).Next(50, 100);
                                Console.WriteLine($@"Sleeping for {sleep}. Page {iPage}. is finished");
                                _logger.Info($"Sleeping for {sleep}. Page {iPage}. is finished");
                                Console.WriteLine($@"Livros {livros.Count} gravados");
                                Thread.Sleep(sleep);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($@"Erro numero --> {Interlocked.Read(ref errorCount)}) <-- na Estante {estante.Nome}, na página {Interlocked.Read(ref iPage)} ERROR: {ex.Message}");
                                Interlocked.Increment(ref errorCount);
                                _logger.Fatal($"Erro fatal na estante {estante.Nome}. Página {iPage}", ex);
                                Console.WriteLine($@"Erro fatal na estante {estante.Nome}. Página {iPage}");
                                if (Interlocked.Read(ref errorCount) > 20)
                                {
                                    producerCancellationToken.Cancel();
                                }
                            }
                        });
                    });

                    Task.WaitAll(producerThread, consumerThread);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _logger.Fatal($"Erro fatal na estante {estante.Nome}.", e);
                }
            }
        }

        private static void ScrapTipoEstantes(EstanteRepository estanteRepository)
        {
            //HtmlWeb web = new HtmlWeb();
            //HtmlDocument document = web.Load("https://www.estantevirtual.com.br/estantes");
            //string[] tiposEstantes = new string[] { "menu-1", "menu-2", "menu-3", "menu-4" };
            //foreach (string tipoEstante in tiposEstantes)
            //{
            //    HtmlNodeCollection estantes =
            //        document.DocumentNode.SelectNodes(
            //            $"//*[@id=\"{tipoEstante}\"]//*[contains(@class, 'list')]/li"
            //        );
            //    List<Estante> estanteCollection = new List<Estante>();
            //    foreach (HtmlNode item in estantes)
            //    {
            //        Estante estante = new Estante();
            //        estante.Nome = item.SelectSingleNode("a").InnerText;
            //        // Para evitar duplicidade, se já houver no banco, pula para o próximo.
            //        if (estanteRepository.Exists(estante.Nome)) { continue; }

            //        estante.Link = item.SelectSingleNode("a").Attributes["href"].Value;
            //        estanteCollection.Add(estante);
            //    }
            //    if (estanteCollection.Count > 0)
            //    {
            //        estanteRepository.AddRange(estanteCollection);
            //        estanteRepository.Save();
            //    }
            //}
        }

        private static void CreateDatabase()
        {
            Database.SetInitializer<ScrapaContext>(null);
        }
    }
}
