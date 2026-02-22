using Laiye.Customer.WebApi.Model.Dto;
using Newtonsoft.Json;
using System.Text;

namespace Laiye.Customer.WebApi.Client
{
    public class ReportInfoClient : BaseClient
    {
        //？？？
        //private ILogger Logger { get; }
        public  ReportInfoClient(ICache cache, string xgatewayApikey, string xauthProduct, string commanderUrl, string headersKey) :
            base(cache, xgatewayApikey, xauthProduct, commanderUrl, headersKey)
        {

        }

        public bool ReportPostInfo(string InputStr)
        {
            //var url = $"{CommanderUrl}/{route}?id=" + projectId;
            var url = CommanderUrl;
            //var url = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api/project?id="+projectId;
            Console.WriteLine(url);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("X-Gateway-Apikey", this.XGatewayApikey);
            HttpClient.DefaultRequestHeaders.Add("X-Auth-Product", this.XAuthProduct);

            var content=new StringContent(InputStr);
            content.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (
                var response = HttpClient.PostAsync(url, content).GetAwaiter().GetResult().EnsureSuccessStatusCode()) 
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resp = JsonConvert.DeserializeObject<BaseResponse2>(result);
                //Logger.LogInformation($"url:{url},content:{content},resp:{resp.code},resp:{resp.message}");
                 return (resp.code == 0);                     
            }
        }

        public bool ReportPutInfo(string InputStr)
        {
            var url = CommanderUrl;
            //var url = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api/project?id="+projectId;
            Console.WriteLine(url);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("X-Gateway-Apikey", this.XGatewayApikey);
            HttpClient.DefaultRequestHeaders.Add("X-Auth-Product", this.XAuthProduct);

            var content = new StringContent(InputStr);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (
                //var response = HttpClient.PutAsJsonAsync(url, content).GetAwaiter().GetResult().EnsureSuccessStatusCode())
                var response = HttpClient.PutAsync(url, content).GetAwaiter().GetResult().EnsureSuccessStatusCode())
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resp = JsonConvert.DeserializeObject<BaseResponse2>(result);
                //Logger.LogInformation($"url:{url},content:{content},resp:{resp.code},resp:{resp.message}");
                return (resp.code == 0);          
            }
        }

        public  bool ReportPutInfo2(string InputStr)
        {
            var url = CommanderUrl;
            //var url = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api/project?id="+projectId;
            Console.WriteLine(url);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("X-Gateway-Apikey", this.XGatewayApikey);
            HttpClient.DefaultRequestHeaders.Add("X-Auth-Product", this.XAuthProduct);

            var content = new StringContent(InputStr);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (
                //var response = HttpClient.PutAsJsonAsync(url, content).GetAwaiter().GetResult().EnsureSuccessStatusCode())
                var response = HttpClient.PutAsync(url, content).GetAwaiter().GetResult().EnsureSuccessStatusCode())
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resp = JsonConvert.DeserializeObject<BaseResponse2>(result);
                //Logger.LogInformation($"url:{url},content:{content},resp:{resp.code},resp:{resp.message}");
                return (resp.code == 0);
            }
        }


        public async Task<bool> ReportDeleteInfo(string InputStr)
        {
            var url = CommanderUrl;
            //var url = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api/project?id="+projectId;
            Console.WriteLine(url);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("X-Gateway-Apikey", this.XGatewayApikey);
            HttpClient.DefaultRequestHeaders.Add("X-Auth-Product", this.XAuthProduct);

            using (var request = new HttpRequestMessage(HttpMethod.Delete, url)) 

            using (request.Content = new StringContent(InputStr, Encoding.UTF8, "application/json"))
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resp = JsonConvert.DeserializeObject<BaseResponse2>(result);
                //Logger.LogInformation($"url:{url},resp:{resp.code},resp:{resp.message}");
                return (resp.code == 0);             
            }
        }
    }
}
