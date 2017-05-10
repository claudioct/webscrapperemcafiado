using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.UI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Books.UI.Controllers
{
    public class StateController : Controller
    {
        private string cacheKey = "livrosCache";
        private string isRecuperandoKey = "isRecuperandoKey";
        private string errorKey = "errorCollection";
        private IMemoryCache _cache;

        public StateController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public JsonResult Index()
        {
            var errors = new Dictionary<DateTime, string>();

            if (_cache.TryGetValue(errorKey, out Dictionary<DateTime, string> outErrors))
            {
                errors = outErrors;
            }

            _cache.TryGetValue(isRecuperandoKey, out bool isValueRecuperandoKey);
            _cache.TryGetValue(cacheKey, out object booksCacheValue);

            var collection =  booksCacheValue as List<LivroEntity>;
            if (collection != null)
            {
                booksCacheValue = collection.Count;
            }

            return Json(new {errors = errors.OrderByDescending(dic => dic.Key), isValueRecuperandoKey, isRecuperandoKey = isValueRecuperandoKey, booksCache = booksCacheValue });
        }
    }
}
