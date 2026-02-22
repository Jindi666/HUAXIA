namespace Laiye.Customer.WebApi.Model.Dto
{
    using Newtonsoft.Json;
    public class BaseResponse { 

        public BaseResponse(string Code="200", string Message= "SUCCESS") //, params string[]messageArguments

        {
         code = Code;
         message = Message;
            
            //messageArguments = messageArguments;

        }

        public string code { get; set; } = "200";

        public string message { get; set; } = "SUCCESS";
       // [JsonIgnore]
      // public string[] messageArguments { get; set; } = new string[0];

    }

    public class BaseResponse<T> : BaseResponse
    {
        public T data { get; set; }

        public BaseResponse(T _data = default(T), string code = "200",  string message = "SUCCESS") : base(code, message)//, params string[] arguments
        {
            data = _data;
        }

    }








    public class ResponeSuccess
    {
        /// <summary>
        /// 返回的URL地址
        /// </summary>
        public string? saasUrl { get; set; }
        /// <summary>
        /// 系统生成的租户ID
        /// </summary>
        public long companyId { get; set; }
    }

    public class ResponeSuccess2
    {
        /// <summary>
        /// 返回的URL地址

        /// </summary>
        public string? loginUrl { get; set; }
        public string? saasUrl { get; set; }
       
       
        /// <summary>
        /// 系统生成的租户ID
        /// </summary>
       
    }








}
