namespace Laiye.Customer.WebApi.Client
{
    public abstract class BaseClient
    {
        private static object LockObject = new object();

        public ICache Cache { get; set; }

        public string HeadersKey { get; set; }

       
        public string CommanderUrl { get; set; }

        public string XAuthProduct {get;set;}

        public string XGatewayApikey { get; set; }

        public static HttpClient HttpClient { get; } = new HttpClient();

        public BaseClient(ICache cache, string xgatewayApikey, string xauthProduct, string commanderUrl, string headersKey)
        {
            Cache = cache;
            XGatewayApikey = xgatewayApikey;
            XAuthProduct = xauthProduct;
            CommanderUrl = commanderUrl;
            HeadersKey = headersKey;
           
         }

    }
}
