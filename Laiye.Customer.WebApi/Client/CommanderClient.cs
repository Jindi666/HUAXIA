using Laiye.Customer.WebApi.Client;
using LazyCache.Providers;

namespace Laiye.Customer.WebApi.Client
{
    public class CommanderClient : IDisposable
    {
      

        private ICacheProvider CacheProvider { get; }

        private ICache Cache { get; }
        public GetProjectNameClient getProjectNameClient { get; }

        public  ReportInfoClient reportInfoClient { get; }

        public CommanderClient(CommanderClientOption options, int type)
        {
            CacheProvider = options.CacheProvider ?? new MemoryCacheProvider();
            Cache = CacheProvider.CreateCache();
            if (type == 1) 
            {
                getProjectNameClient = new GetProjectNameClient(Cache, "", "", options.CommanderUrl, options.HeadersKey);
            }
            if (type == 2) 
            {
                Console.WriteLine("XGatewayApikey:"+ options.XGatewayApikey);
                Console.WriteLine("XAuthProduct:"+ options.XAuthProduct);
                Console.WriteLine("CommanderUrl:"+ options.CommanderUrl);
                reportInfoClient = new ReportInfoClient(Cache,
                                                        options.XGatewayApikey,
                                                        options.XAuthProduct,
                                                        options.CommanderUrl,
                                                        "");
            }
        }
        

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
