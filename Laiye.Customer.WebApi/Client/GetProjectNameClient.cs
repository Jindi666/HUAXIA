using Laiye.Customer.WebApi.Model.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Laiye.Customer.WebApi.Client
{
    public class GetProjectNameClient : BaseClient
    {
        private string[] args { get; }
        private IConfiguration Configuration { get; }
        private ICache _cache { get; }
        private string _xgatewayApikey { get; }
        private string _xauthProduct { get; }
        private string _commanderUrl { get; }
        private string _headersKey { get; }
        private HttpClient _httpClient { get; set; }

        public GetProjectNameClient(ICache cache, string xgatewayApikey, string xauthProduct, string commanderUrl, string headersKey) : 
            base(cache, xgatewayApikey,  xauthProduct, commanderUrl, headersKey)
        {
            _cache = cache;
            _xgatewayApikey = xgatewayApikey;
            _xauthProduct = xauthProduct;
            _commanderUrl = commanderUrl;
            _headersKey = headersKey;
            _httpClient = new HttpClient();
        }
        

        
        public string[] GetProjectNameAsync(string projectId)
        {
            var route = "project";
            var url = $"{CommanderUrl}/{route}?id=" + projectId;
            //var url = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api/project?id="+projectId+;
            //Console.WriteLine(url);
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            System.Net.Http.Headers.HttpRequestHeaders defaultRequestHeaders = _httpClient.DefaultRequestHeaders;
            defaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-gateway-apikey", this.HeadersKey);

            using (
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult().EnsureSuccessStatusCode())
            //var response = responseMessage.GetAwaiter().GetResult().EnsureSuccessStatusCode();
            {

                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //Console.WriteLine("********************result:"+ result);

                var resp = JsonConvert.DeserializeObject<getProjectNameResponse<List<ProjectNameSuccess>>>(result);
                //Console.WriteLine("********************resp.code:" + resp.code);
                if (resp.code == 200)
                {
                    if (resp.Data.Count > 0)
                    {
                        string[] names = new string[4] { resp.Data[0].code, resp.Data[0].name, resp.Data[0].tenantId, resp.Data[0].tenantName };
                        //if (type == 1) { return resp.Data[0].name;}
                        //else  { return resp.Data[0].code;}

                        return names;
                    }
                    else
                    {

                        return new string[2] { "error", "error" };
                    }

                }
                else return new string[2] { "error", "error" };
            }



            //using (


            //   // string retString = await HttpClient.GetStringAsync(url)
            //   var response = HttpClient.GetAsync(url).GetAwaiter().GetResult().EnsureSuccessStatusCode()

            //    )

            //    {   var result =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //        //Console.WriteLine("********************result:"+ result);

            //        var resp= JsonConvert.DeserializeObject<getProjectNameResponse<List<ProjectNameSuccess>>>(result);
            //        //Console.WriteLine("********************resp.code:" + resp.code);
            //        if (resp.code == 200) {
            //            if (resp.Data.Count > 0) 
            //            {
            //            string[] names = new string[4] { resp.Data[0].code, resp.Data[0].name,resp.Data[0].tenantId,resp.Data[0].tenantName };
            //            //if (type == 1) { return resp.Data[0].name;}
            //            //else  { return resp.Data[0].code;}

            //            return  names ;
            //            }
            //            else
            //            {

            //            return new string[2] { "error", "error" };
            //            }

            //        } else return new string[2] { "error", "error" };
            //    //return result;
            //}

        }
        
    }
}
