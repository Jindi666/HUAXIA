namespace Laiye.Customer.WebApi.Model.Dto
{
    public class getProjectNameResponse
    {
        public getProjectNameResponse()
        {
            total = total;
            code = code;
            message = message;
           

        }
        public int code { get; set; } = 200;
        public int total { get; set; }
        public string message { get; set; } = "SUCCESS";

    }


    public class getProjectNameResponse<T> : getProjectNameResponse
    {
        public T Data { get; set; }

        public getProjectNameResponse(T data = default(T), int code = 0, string message = "", params string[] arguments)
        {
            Data = data;
        }
    }

    public class ProjectNameSuccess
    {
        /// <summary>
        /// 返回的URL地址
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 系统生成的租户ID
        /// </summary>
       public string code { get; set; }

       public string tenantId { get; set; }

        public string tenantName { get; set; }


    }

    public class ContentItem
    { 
        
    }
}
