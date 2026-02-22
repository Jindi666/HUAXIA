using Microsoft.AspNetCore.Mvc;

namespace Laiye.Customer.WebApi.Model
{
    public class ZsyHeaders
    {
        [FromHeader]
        public string authToken { get; set; }
    }
}
