namespace Laiye.Customer.WebApi.Client
{

    public class MemoryCacheProvider : ICacheProvider
    {
        public ICache CreateCache()
        {
            return new MemoryCache();
        }
    }
}
