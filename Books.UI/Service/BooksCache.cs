using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.UI.Data;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Books.UI.Service
{
    public static class BooksCache
    {
        private static string cacheKey = "livrosCache";
        private static string isRecuperandoKey = "isRecuperandoKey";
        private static string errorKey = "errorCollection";

        public static void Recuperar(IMemoryCache cache)
        {
            var cacheEntryOptionsRecuperando = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(3));

            cache.TryGetValue(isRecuperandoKey, out bool isRecuperandoCollection);

            
            if (isRecuperandoCollection == false)
            {
                Task task = Task.Run(() =>
                {
                        cache.Set(isRecuperandoKey, true, cacheEntryOptionsRecuperando);

                        var livrosCollection = new BooksDao().GetAllBooks();

                        var cacheEntryOptionsCollection = new MemoryCacheEntryOptions()
                            .SetPriority(CacheItemPriority.NeverRemove);

                        cache.Set(cacheKey, livrosCollection, cacheEntryOptionsCollection);

                        cache.Set(isRecuperandoKey, false, cacheEntryOptionsRecuperando);
                });
                task.ContinueWith(
                    t =>
                    {
                        var ex = t.Exception;
                        if (ex != null)
                        {
                            cache.Set(isRecuperandoKey, false, cacheEntryOptionsRecuperando);

                            var errorCacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromHours(1));

                            var errors = new Dictionary<DateTime, string>();
                            if (cache.TryGetValue(errorKey, out Dictionary<DateTime, string> outErrors))
                            {
                                errors = outErrors;
                            }
                            errors.Add(DateTime.UtcNow, ex.ToString());

                            cache.Set(errorKey, errors, errorCacheEntryOptions);
                        }
                    });
            }
        }
    }
}
