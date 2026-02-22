namespace Laiye.Customer.WebApi.Client
{
    public class CommanderClientOption
    {
       
        public string XGatewayApikey { get; set; }
        public string XAuthProduct { get; set; }
       
        public string CommanderUrl { get; set; }

        public string HeadersKey { get; set; }
        public ICacheProvider CacheProvider { get; set; }

    }
}
