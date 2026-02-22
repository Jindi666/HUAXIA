using Google.Protobuf.WellKnownTypes;
using Laiye.Customer.WebApi.Client;
using Laiye.Customer.WebApi.Model;
using Laiye.EntUC.Core.Event;
using Newtonsoft.Json;
using static Laiye.EntCmd.Service.Core.Worker;
using static Laiye.EntUC.Service.Tenant.Employee;
using static Laiye.EntUC.Service.User.User;

namespace Laiye.Customer.WebApi.TimedExecutService
{
    public class TimedExecutService 

    {
        public ILogger _logger;
       
        public WorkerClient _WorkerClient { get; }
        public EmployeeClient _EmployeeClient { get; }

        public UserClient _UserClient { get; }
        public IFreeSql _conn { get; }

        private IConfiguration _Configuration { get; }
   

        public TimedExecutService(ILogger logger)
        {
            _logger = logger;

        }

      

        public  async Task WorkerDataUpProcessAsync()
        {

            var response = await _WorkerClient.GetWorkersAsync(new EntCmd.Service.Core.GetWorkersRequest()
            {
                GroupId = -1
            });

            if (response.Workers != null && response.Workers.Count > 0)
            {

                foreach (var item in response.Workers)
                {
                    var workid = item.Id;
                    var companyid = item.CompanyId;
                    var args = new CommonEventArgs();
                    args.CompanyId = companyid;
                    args.Id = workid;
                    WorkerUpdate(args);
                    Thread.Sleep(50);


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

            string requestinfo = await doWorkerInfo(2, args);
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = _Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = _Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = _Configuration["worker_info"],

            }, 2);

            _logger.LogInformation($"quartz:Workerupdate  requestioninfo is :  {requestinfo}");
            var ret = client.reportInfoClient.ReportPutInfo(requestinfo);
            if (ret == false)
            {
                _logger.LogInformation("quartz:Workerupdate.ReportPostInfo failed");
            };
        }


        public SubscriptionModel QueryWorkSubscripInfo(IFreeSql conn1, long companyId)
        {

            //var maxid = conn1.Select<SubscriptionModel>().Where(a => a.Companyid == companyId ).OrderBy(a=>a.Id).ToList() ;

            //加上lordertype=2
            var query1 = conn1.Select<SubscriptionModel>().Where(a => a.Companyid == companyId && a.Ordertype == 2).ToList();
            query1.OrderByDescending(a => a.Id);

            if (query1 != null)
            {
                return query1.First();
            }
            else
                return null;


        }


        private RelationshipModel QueryTenantInfo(IFreeSql conn, long companyid)
        {

            var Query = conn.Select<RelationshipModel>().Where(a => a.CompanyId == companyid).ToList();//Where(a=>a.CompanyId ==companyid);
            if (Query.Count == 0)
                return null;
            return Query.First();
        }



        public async Task<string> doWorkerInfo(int status, CommonEventArgs args)
        {
            //status = 1:  add;     2: update;     3: delete    4: 状态上报
            _logger.LogInformation($"quartz:Workerupdate  《3.{status}》  GetWorkerByIdAsync begin");

            var worker_resp = new EntCmd.Service.Core.GetWorkerByIdResponse();

            if (status == 3)
            {
                var worker_resp1 = await _WorkerClient.GetDeletedWorkerByIdAsync(new EntCmd.Service.Core.GetWorkerByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                worker_resp = worker_resp1;
                _logger.LogInformation($"quartz:Workerupdate  《3.{status}》  delete");

            }
            else
            {
                var worker_resp2 = await _WorkerClient.GetWorkerByIdAsync(new EntCmd.Service.Core.GetWorkerByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                worker_resp = worker_resp2;
                _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  QueryInfo");

            }

            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》 GetWorkerByIdAsync{worker_resp}");
            long id = worker_resp.Worker.Id; //workerid
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  id:{id}");
            string workname = worker_resp.Worker.WorkerName;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  workname:{workname}");
            string ip = worker_resp.Worker.LastLoginIp;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  ip:{ip}");

            var workinfo_resp = QueryWorkSubscripInfo(_conn, args.CompanyId);

            var tmpTimestamp = DateTime.UtcNow; //- DateTime.Now;
            string license = workinfo_resp == null ? "无授权" : "浮动授权";
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  license:{license}");
            DateTime ls_begintime = workinfo_resp != null ? workinfo_resp.Ls_begintime : tmpTimestamp;//License 开始时间
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  ls_begintime:{ls_begintime}");
            DateTime Ls_endtime = workinfo_resp != null ? workinfo_resp.Ls_endtime : tmpTimestamp;//License结束时间
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  Ls_endtime:{Ls_endtime}");
            string OrderNo_zsy = workinfo_resp.OrderNo_zsy;//worker订单号
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  OrderNo_zsy:{OrderNo_zsy}");
            int workertype = worker_resp.Worker.IsAccept;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  workertype:{ workertype}");
            int workerstatus = worker_resp.Worker.IsOnline;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  workerstatus:{workerstatus}");

            //要记得修改ent_cmd service的领域参数，现在默认带了
            WorkerUpdatedEventArgs extArgs = null;

            if (status != 1 && status != 4)
            {
                _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  rstatus 2,3:{status}");
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
                        _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  case1");
                        break;
                    }
                case 2:
                    {
                        operatetime = extArgs.updateTime.ToTimestamp();
                        employee_id = extArgs.updateEmployeeId;
                        _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  case2");
                        break;
                    }
                case 3:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;
                        _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  case3");
                        break;
                    }
                case 4:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;

                        _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  case4");
                        break;
                    }
            }

            var employee_resp = _EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
            {
                EmployeeId = employee_id,
                CompanyId = args.CompanyId//
            });



            string username = employee_resp.Employee.UserName;

            var user_resp1 = _UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp1.User.SourceValue;



            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  username{ username}");
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》 GetEmployeeByUserId{employee_resp}");


            //查询方法
            var relationInfo_resp = QueryTenantInfo(_conn, args.CompanyId);
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》QueryTenantInfo{relationInfo_resp}");

            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  TenantId_zsy{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  Projectid{Projectid}");
            string strContent = "";

            if (status == 1)

                strContent = JsonConvert.SerializeObject(new ReportAddWorkerInfo
                {
                    id = Convert.ToString(id),
                    tenantId = TenantId_zsy,
                    //tenantId = "3e7780189e2545fe9de49e08e1e2f678",
                    projectId = Projectid,
                    license = license,
                    orderId = OrderNo_zsy,

                    begin = new DateTimeOffset(workinfo_resp?.Ls_begintime ?? tmpTimestamp).ToUnixTimeMilliseconds(),
                    end = new DateTimeOffset(workinfo_resp?.Ls_endtime ?? tmpTimestamp).ToUnixTimeMilliseconds(),

                    ip = string.IsNullOrEmpty(ip) ? "0.0.0.0" : ip,
                    name = workname,
                    type = workertype,
                    creatorCode = username,
                    createTime = operatetime.ToDateTimeOffset().ToUnixTimeSeconds() * 1000

                });

            if (status == 2 || status == 3)
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
                    creatorCode = creatorCode, //worker_resp.Worker.CreateEmployee,
                    createTime = worker_resp.Worker.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                    updatorCode = username,
                    //updatorCode = "60056455",
                    updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                });
            }
               

            if (status == 4)
            {
                strContent = JsonConvert.SerializeObject(new ReportStatusWorkerInfo
                {
                    id = Convert.ToString(id),
                    tenantId = TenantId_zsy,
                    projectId = Projectid,
                    status = workerstatus
                });
                _logger.LogInformation($"quartz:Workerupdate 《3.{status}》  status {status}");
            }

            _logger.LogInformation($"quartz:Workerupdate 《3.{status}》strContent{strContent}");
            return strContent;
        }

    }
}
