using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Laiye.Customer.WebApi.Client
{
    public class MemoryCache : ICache, IDisposable
    {
        private const string CacheKey = "_uibot_commander_access_token";

        private Microsoft.Extensions.Caching.Memory.MemoryCache InnerCache { get; } = new Microsoft.Extensions.Caching.Memory.MemoryCache(Options.Create(new MemoryCacheOptions
        {

        }));

        public void Dispose()
        {
            InnerCache.Dispose();
        }

        public string Get()
        {
            return InnerCache.Get<string>(CacheKey);
        }

        public void Set(string value, TimeSpan? timestemp = null)
        {
            InnerCache.Set(CacheKey, value, timestemp.GetValueOrDefault(TimeSpan.FromHours(1)));
        }
    }
}

