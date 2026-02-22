namespace Laiye.Customer.WebApi.Client
{
    public interface ICache : IDisposable
    {
        string Get();
        void Set(string value, TimeSpan? timestemp = null);
    }
}
