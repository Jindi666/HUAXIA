using System.Text.Json.Serialization;

namespace Laiye.Customer.WebApi.Model.Dto
{
    public class BaseResponse2
    {
        public BaseResponse2(int _code = 0, string _message = "SUCCESS", params string[] _messageArguments)

        {
            code = _code;
            message = _message;
            messageArguments = _messageArguments;

        }

        public int code { get; set; } =0;

        public string message { get; set; } = "SUCCESS";
        [JsonIgnore]
        public string[] messageArguments { get; set; } = new string[0];

    }

    public class BaseResponse2<T> : BaseResponse
    {
        public T Data { get; set; }

        public BaseResponse2(T data = default(T), int code =0, string message = "", params string[] arguments)
        {
            Data = data;
        }
    }

}

