using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scrapa.Services
{
    public class PriceStatisticsService
    {

        public PriceStatistics Get(long id)
        {
            var response = Execute(id);

            if (string.IsNullOrWhiteSpace(response?.Book_id))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(response?.Stats?.Max))
            {
                return null;
            }


            PriceStatistics priceStatistics = new PriceStatistics()
            {
                QtdVendida = response.Stats.Count,
                DtUltimaVenda = response.Stats.LastSold.Date,
                ValorUltimaVenda = response.Stats.LastSold.Price,
                ValorMaxVenda = response.Stats.Max,
                ValorMinVenda = response.Stats.Min,
                ValorMedioVenda = response.Stats.mean
            };


            return priceStatistics;
        }

        private PriceModel Execute(long id)
        {
            var response = ExecuteRequest(id);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                for (int i = 0; i < 10; i++)
                {
                    response = ExecuteRequest(id);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.Data;
                    }
                    Thread.Sleep(2000);
                }
            }

            return response.Data;
        }

        private IRestResponse<PriceModel> ExecuteRequest(long id)
        {
            var client = new RestClient("https://www.estantevirtual.com.br/");
            var request = new RestRequest("/mod_perl/get_price_stats.cgi", Method.GET);
            request.AddParameter("book_id", id);

            var response = client.Execute<PriceModel>(request);
            return response;
        }
    }
}
