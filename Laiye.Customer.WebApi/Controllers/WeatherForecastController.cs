using DotNetCore.CAP;
using Laiye.Customer.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Cryptography;
using static Laiye.EntUC.Service.Tenant.Tenant;

namespace Laiye.Customer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get(
            [FromBody]TestModel request,
            [FromServices]IFreeSql conn,
            [FromServices]TenantClient tenantClient)
        {

            conn.Transaction(() =>
            {
                conn.Insert(new TestModel { }).ExecuteAffrows();
                conn.Insert(new TestModel { }).ExecuteAffrows();
                conn.Insert(new TestModel { }).ExecuteAffrows();
                conn.Insert(new TestModel { }).ExecuteAffrows();
            });

            var resp = await tenantClient.CreateTenantAsync(new EntUC.Service.Tenant.CreateTenantRequest
            {
                AdministratorEmployeeId = 1,
            });
            // 邀请的UserId写死为0
            // 邀请的UserName写死为空
            // resp.Id

            conn.Insert(new TestModel { }).ExecuteAffrows();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        public class EvnetArgs
        {
            public long Id { get; set; }
        }

        [HttpPost("createSub")]
        public async Task CreateSub([FromServices]ICapPublisher publisher)
        {
            // 插入数据库创建订阅记录 处理中

            await publisher.PublishAsync("zsj.createSub", new EvnetArgs { Id = 11 });

        }



        private static Random Random = new Random();

        //[CapSubscribe("szj.createSub")]
        //public async Task ProcessSub(EventArgs args)
        //{
            //grpc 是否要创建租户

            //using (var hacm = HMACSHA256.Create())
            //{
            //     //hacm.ComputeHash()
            //}


            //using (var aes = Aes.Create())
            //{
            //    aes.Mode = CipherMode.CBC;
            //    Convert.FromBase64String()
            //}

            //Random.Next(,)

            //JsonConvert.SerializeObject(new { });

            //JsonConvert.DeserializeObject<TestModel>("");

            // 生成邀请链接

            // 更新数据库的订阅记录状态为已完成
       //}
    }
}