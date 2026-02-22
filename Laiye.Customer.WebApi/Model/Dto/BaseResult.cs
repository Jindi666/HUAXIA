using Newtonsoft.Json;


namespace Laiye.Customer.WebApi.Model.Dto
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class BaseResult
    {
        public BaseResult(int code = 200, string message = "", params string[] messageArguments)
        {
            Code = code;
            Message = message;
            MessageArguments = messageArguments;
        }

        /// <summary>
        /// 返回结果代码 200为正常 其他为异常
        /// </summary>
        public int Code { get; set; } = 200;

        public string Message { get; set; } = string.Empty;

        [JsonIgnore]
        public string[] MessageArguments { get; set; } = new string[0];
    }

    /// <summary>
    /// 带数据返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResult<T> : BaseResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        public BaseResult(T data = default(T), int code = 200, string message = "", params string[] arguments) : base(code, message, arguments)
        {
            Data = data;
        }
    }

    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedData<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T[] Items { get; set; }

        /// <summary>
        /// 所有数据总条数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
    }

}
