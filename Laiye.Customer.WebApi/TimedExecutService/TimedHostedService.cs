using Google.Protobuf.WellKnownTypes;
using Laiye.Customer.WebApi.Client;
using Laiye.Customer.WebApi.Model;
using Laiye.EntUC.Core.Event;
using Newtonsoft.Json;
using static Laiye.EntCmd.Service.Core.Worker;
using static Laiye.EntUC.Service.Tenant.Employee;
using static Laiye.EntUC.Service.Tenant.Tenant;
using static Laiye.EntUC.Service.User.User;

namespace Laiye.Customer.WebApi.TimedExecutService
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer? _timer = null;

        public WorkerClient _WorkerClient { get; }
        public EmployeeClient _EmployeeClient { get; }

        public TenantClient _TenantClient { get; }

        public UserClient _UserClient { get; }

        public IFreeSql _conn { get; }

        private IConfiguration _Configuration { get; }

        public TimedHostedService(ILogger<TimedHostedService> logger, WorkerClient workerClient, EmployeeClient employeeClient, TenantClient tenantClient, UserClient userClient,IFreeSql conn, IConfiguration configuration)
        {
            _logger = logger;
            _WorkerClient = workerClient;
            _EmployeeClient = employeeClient;
            _conn = conn;
            _TenantClient = tenantClient;
            _Configuration = configuration;
            _UserClient = userClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromHours(1));
           // _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            //判断还是不是1点
            if (DateTime.Now.Hour == 1)
            {

                _logger.LogInformation(
                    "Timed Hosted Service is working. Count: {Count}", 1);

                await WorkerDataUpProcessAsync();
            }
           // var count = Interlocked.Increment(ref executionCount);

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

                     


        public async Task WorkerDataUpProcessAsync()
        {

           
            var resoponse = await _TenantClient.GetTenantsAsync(new EntUC.Service.Tenant.GetTenantsRequest()
            {
                //Keyword = "",
                //PageIndex = 1,
                //PageSize=100

            }) ;
            
            if (resoponse.Tenants!=null && resoponse.Tenants.Count > 0)
            {
                foreach (var items1 in resoponse.Tenants)
                {
                   if (items1.IsEnabled==true)
                    {
                        var ret = await _WorkerClient.GetWorkersAsync(new EntCmd.Service.Core.GetWorkersRequest()
                        {
                            //GroupId = -1
                            CompanyId = items1.Id

                        });

                        if (ret.Workers != null && ret.Workers.Count > 0)
                        {
                            foreach (var item in ret.Workers)
                            {
                                
                                var workid = item.Id;
                                _logger.LogInformation($"UpWorkerInfo  workid:{workid}");
                                var companyid = item.CompanyId;
                                _logger.LogInformation($"UpWorkerInfo  companyid:{companyid}");
                                var args = new CommonEventArgs();
                                args.CompanyId = companyid;
                                args.Id = workid;
                                await WorkerUpdate(args);
                                Thread.Sleep(50);

                            }
                        }

                    }
                    

                }
            }

           
        }

        private class WorkerUpdatedEventArgs
        {
            public long NewMachineId { get; set; }

            public long OldMachineId { get; set; }

            public bool NeedReboot { get; set; }

            public DateTime updateTime { get; set; }

            public long updateEmployeeId { get; set; }



        }

        public async Task WorkerUpdate(CommonEventArgs args)
        {

            _logger.LogInformation("UpWorkerInfo  begin........ ");
            string requestinfo = await doWorkerInfo(2, args);

            //var client = new CommanderClient(new CommanderClientOption
            //{
            //    XAuthProduct = _Configuration["X-Auth-Product"],//取文件
            //    XGatewayApikey = _Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
            //    CommanderUrl = _Configuration["worker_info"],

            //}, 2);
            //_logger.LogInformation("UpWorkerInfo  http sendmessage.... ");
            //_logger.LogInformation($"UpWorkerInfo  requestioninfo is :  {requestinfo}");
            //var ret = await client.reportInfoClient.ReportPutInfo(requestinfo);
            //if (ret == false)
            //{
            //    _logger.LogInformation("UpWorkerInfo.ReportPostInfo failed");
            //};
        }


        public async Task<SubscriptionModel> QueryWorkSubscripInfo(IFreeSql conn1, long companyId)
        {

            //var maxid = conn1.Select<SubscriptionModel>().Where(a => a.Companyid == companyId ).OrderBy(a=>a.Id).ToList() ;

            //加上lordertype=2
            var query1 = await conn1.Select<SubscriptionModel>().Where(a => a.Companyid == companyId && a.Ordertype == 2).ToListAsync();//.ToList();
            query1.OrderByDescending(a => a.Id);

            if (query1 != null)
            {
                return query1.First();
            }
            else
                return null;


        }


        private async Task<RelationshipModel> QueryTenantInfo(IFreeSql conn, long companyid)
        {

            var Query = await conn.Select<RelationshipModel>().Where(a => a.CompanyId == companyid).ToListAsync();//.ToList();//Where(a=>a.CompanyId ==companyid);
            if (Query.Count == 0)
                return null;
            return Query.First();
        }



        public async Task<string> doWorkerInfo(int status, CommonEventArgs args)
        {
            //status = 1:  add;     2: update;     3: delete    4: 状态上报
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  GetWorkerByIdAsync begin");

            //var worker_resp = new EntCmd.Service.Core.GetWorkerByIdResponse();

           
            
                var worker_resp = await _WorkerClient.GetWorkerByIdAsync(new EntCmd.Service.Core.GetWorkerByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
               // worker_resp = worker_resp2;
                _logger.LogInformation($"UpWorkerInfo 《1.{status}》  QueryInfo");

            

            _logger.LogInformation($"UpWorkerInfo 《1.{status}》 GetWorkerByIdAsync{worker_resp}");
            long id = worker_resp.Worker.Id; //workerid
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  id:{id}");
            string workname = worker_resp.Worker.WorkerName;
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  workname:{workname}");
            string ip = worker_resp.Worker.LastLoginIp;
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  ip:{ip}");

            var workinfo_resp =await QueryWorkSubscripInfo(_conn, args.CompanyId);

            var tmpTimestamp = DateTime.UtcNow; //- DateTime.Now;
            string license = workinfo_resp == null ? "无授权" : "浮动授权";
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  license:{license}");
            DateTime ls_begintime = workinfo_resp != null ? workinfo_resp.Ls_begintime : tmpTimestamp;//License 开始时间
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  ls_begintime:{ls_begintime}");
            DateTime Ls_endtime = workinfo_resp != null ? workinfo_resp.Ls_endtime : tmpTimestamp;//License结束时间
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  Ls_endtime:{Ls_endtime}");
            string OrderNo_zsy = workinfo_resp.OrderNo_zsy;//worker订单号
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  OrderNo_zsy:{OrderNo_zsy}");
            int workertype = worker_resp.Worker.IsAccept;
            _logger.LogInformation($"doWorkerInfo 《1.{status}》  workertype:{ workertype}");
            int workerstatus = worker_resp.Worker.IsOnline;
            _logger.LogInformation($"doWorkerInfo 《1.{status}》  workerstatus:{workerstatus}");

            //要记得修改ent_cmd service的领域参数，现在默认带了
            WorkerUpdatedEventArgs extArgs = null;

            if (status != 1 && status != 4)
            {
                _logger.LogInformation($"UpWorkerInfo 《1.{status}》  rstatus 2,3:{status}");
                extArgs = JsonConvert.DeserializeObject<WorkerUpdatedEventArgs>(args.EventArgs);
            }


            Timestamp operatetime = null;
            long employee_id = 0;

            switch (status)
            {
                case 1:
                    {
                        operatetime = worker_resp.Worker.CreateTime;
                        employee_id = worker_resp.Worker.CreateEmployeeId;
                        _logger.LogInformation($"UpWorkerInfo 《1.{status}》  case1");
                        break;
                    }
                case 2:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp(); 
                        employee_id = worker_resp.Worker.CreateEmployeeId; //extArgs.updateEmployeeId;
                        _logger.LogInformation($"UpWorkerInfo 《1.{status}》  case2");
                        break;
                    }
                case 3:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;
                        _logger.LogInformation($"UpWorkerInfo 《1.{status}》  case3");
                        break;
                    }
                case 4:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;

                        _logger.LogInformation($"UpWorkerInfo 《1.{status}》  case4");
                        break;
                    }
            }

            var employee_resp = _EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
            {
                EmployeeId = employee_id,
                CompanyId = args.CompanyId
            });
            string username = employee_resp.Employee.UserName;

            var user_resp2 = _UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp2.User.SourceValue;

            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  username{ username}");
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》 GetEmployeeByUserId{employee_resp}");


            //查询方法
            var relationInfo_resp = await QueryTenantInfo(_conn, args.CompanyId);
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》QueryTenantInfo{relationInfo_resp}");

            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  TenantId_zsy{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            _logger.LogInformation($"UpWorkerInfo 《1.{status}》  Projectid{Projectid}");
            string strContent = "";


            if (status == 2)
            {
                var employee_resp1 = _EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
                {
                    EmployeeId = worker_resp.Worker.CreateEmployeeId,
                    CompanyId = args.CompanyId//
                });


                var user_resp3 = _UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                {
                    UserName = employee_resp1.Employee.UserName
                });
                //获取nickname参数
                string creatorCode = user_resp3.User.SourceValue;
                strContent = JsonConvert.SerializeObject(new ReportModifyWorkerInfo
                {
                    id = Convert.ToString(id),
                    tenantId = TenantId_zsy,
                    //tenantId = "3e7780189e2545fe9de49e08e1e2f678",
                    projectId = Projectid,
                    license = license,
                    orderId = OrderNo_zsy,
                    begin = new DateTimeOffset(workinfo_resp?.Ls_begintime ?? tmpTimestamp).ToUnixTimeMilliseconds(),//  ls_begintime,
                    end = new DateTimeOffset(workinfo_resp?.Ls_endtime ?? tmpTimestamp).ToUnixTimeMilliseconds(),
                    ip = string.IsNullOrEmpty(ip) ? "0.0.0.0" : ip,
                    name = workname,
                    type = workertype,
                    status = workerstatus,
                    creatorCode = creatorCode,   //worker_resp.Worker.CreateEmployee,
                    createTime = worker_resp.Worker.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                    updatorCode = username,
                    //updatorCode = "60056455",
                    updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                });
            }
                

           

            _logger.LogInformation($"UpWorkerInfo 《1.{status}》strContent{strContent}");

            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = _Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = _Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = _Configuration["worker_info"],

            }, 2);
            _logger.LogInformation("UpWorkerInfo  http sendmessage.... ");
            _logger.LogInformation($"UpWorkerInfo  requestioninfo is :  {strContent}");
           
            var ret =client.reportInfoClient.ReportPutInfo2(strContent);
            if (ret == false)
            {
                _logger.LogInformation("UpWorkerInfo.ReportPostInfo failed");
            };




            return strContent;
        }





        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
