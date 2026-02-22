using DotNetCore.CAP;
using Dm;
using FreeSql;
using FreeSql.DataAnnotations;
using Hangfire;
using Hangfire.MySql;
using Hangfire.Redis;
using Laiye.Customer.WebApi.CAP;
using Laiye.Customer.WebApi.Filters;
using Laiye.Customer.WebApi.Model;
using Laiye.Customer.WebApi.Utils;
using Laiye.Customer.WebApi.TimedExecutService;
using Laiye.EntCmd.Service.App;
using Laiye.EntCmd.Service.Grpc;
using Laiye.EntUC.Service.Grpc;
using NLog.Extensions.Logging;
using Savorboard.CAP.InMemoryMessageQueue;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using static Laiye.EntCmd.Service.Core.Worker;
using static Laiye.EntUC.Service.Tenant.Employee;






var builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;

// Add services to the container.

var configuration = new ConfigurationBuilder()
               .AddJsonFile(Path.Combine("Configs", "appsettings.json"), true, true).Build();

  

builder.Services.AddControllers();//.AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntUCGrpcClient(options =>
{
    options.ServiceAddress = builder.Configuration["Service:UserCenterAddress"];
});

builder.Services.AddEntCmdGrpcClient(options =>
{
    options.ServiceAddress = builder.Configuration["Service:CommanderAddress"];
});

builder.Services.AddSingleton<IFreeSql>(serviceProvider =>
{
    var dbTypeStr = builder.Configuration["Db:DbType"];
    DataType dbType = DataType.MySql; // 默认MySQL

    if (!string.IsNullOrEmpty(dbTypeStr))
    {
        var normalizedDbType = dbTypeStr.Trim().ToLowerInvariant();

        // 检测到达梦数据库,使用自定义适配器工厂
        if (normalizedDbType == "dameng" || normalizedDbType == "dm")
        {
            Console.WriteLine("检测到达梦数据库，使用自定义适配器模式...");
            return DmFreeSqlFactory.CreateDamengFreeSql(builder.Configuration["Db:Connection"]);
        }

        dbType = (DataType)Enum.Parse(typeof(DataType), dbTypeStr, true);
    }

    var fsql = new FreeSqlBuilder()
        .UseConnectionString(dbType, builder.Configuration["Db:Connection"])
        .UseAutoSyncStructure(false)
        .UseMonitorCommand(cmd => Console.WriteLine($"SQL: {cmd.CommandText}"))
        .UseNoneCommandParameter(true)
        .Build();

    Console.WriteLine($"FreeSql 已创建（版本 3.2.833），数据库类型：{dbType}");

    return fsql;
});

//var hangfireOption = new RedisStorageOptions
//{
//    DeletedListSize = 1024,
//    SucceededListSize = 1024,
//};
//var hangfireinfo = builder.Configuration["Redis:Connection"];
//builder.Services.AddHangfire(r => r.UseRedisStorage(hangfireinfo, hangfireOption));

builder.Services.AddHostedService<TimedHostedService>();


builder.Services.AddSingleton<ICapSubscribe, CapSubscribeService>();




builder.Services.AddCap(options =>
{
    options.UseInMemoryStorage();
    options.UseInMemoryMessageQueue();
    //options.UseRabbitMQ("");
});


builder.Services.AddHealthChecks();

builder.Services.AddLogging(logger =>
{
    logger.ClearProviders();
#if DEBUG
    logger.AddConsole();
#endif
    logger.AddNLog("nlog.config");
   

});



//������
//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add<CustomerActionFilter>();
//});

var app = builder.Build();
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
var options = new BackgroundJobServerOptions
{
    ServerName = $"{Environment.MachineName}_{Process.GetCurrentProcess().Id}"
};

//app.UseHangfireServer(options);
//app.UseHangfireDashboard();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/", async context =>
//    {
//        await context.Response.WriteAsync("Hello World!");
//    });
//});




//await InitDatabase(app.Services); // 暂时注释掉数据库初始化

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ILogger _logger;

//WorkerClient _WorkerClient;
//EmployeeClient _EmployeeClient;
//IFreeSql _conn;

//IConfiguration _Configuration ;
       
        

//var timedExecutService = ;

//GlobalConfiguration.Configuration.UseRedisStorage(builder.Configuration["Redis:Connection"]);
//RecurringJob.AddOrUpdate("WorkerInfoUp ", () => timedExecutService.WorkerDataUpProcessAsync(), Cron.MinuteInterval(2));
//.Daily(17,28)


app.Run();

 


Task InitDatabase(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var tableAttribteType = typeof(TableAttribute);
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var conn = scope.ServiceProvider.GetRequiredService<IFreeSql>();
        var modelType = typeof(IModel);
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(type => modelType.IsAssignableFrom(type) && type != modelType && type.CustomAttributes.Any(attr => attr.AttributeType == tableAttribteType))
            .ToArray();
        logger.LogInformation("databse struct...");
        //using (var conn = rd.GetConnection())
        {
            logger.LogInformation($"database connect str��{conn.Ado.MasterPool.Get(TimeSpan.FromSeconds(5)).Value.ConnectionString}");
            Parallel.ForEach(types, item =>
            {
                var cmd = conn.CodeFirst.GetComparisonDDLStatements(item);
                logger.LogInformation($"databse struct1��{item.FullName}\n{cmd}");
                conn.Ado.CommandFluent(cmd)
                    .CommandTimeout((int)TimeSpan.FromHours(24).TotalSeconds)
                    .ExecuteNonQuery();
                logger.LogInformation($"databse struct2��{item.FullName}complete");
            });
            //foreach (var item in types)
            //{
            //    conn.CodeFirst.SyncStructure(item); 
            //}
            logger.LogInformation($"databse struct2����complete");
        }
    }






    return Task.CompletedTask;
}