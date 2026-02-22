using DotNetCore.CAP;
using static Laiye.EntCmd.Service.Core.Flow;
using static Laiye.EntCmd.Service.Core.Task;
using static Laiye.EntCmd.Service.Core.Worker;
using static Laiye.EntUC.Service.User.User;
using static Laiye.EntUC.Service.Tenant.Employee;
using static Laiye.EntUC.Service.Tenant.Tenant;
using static Laiye.EntUC.Service.Configuration.License;
using Laiye.EntUC.Core.Event;
using Laiye.Customer.WebApi.Model;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;
using Laiye.Customer.WebApi.Client;
using static Laiye.EntCmd.Service.Core.TaskEnum.Types;
using System.Timers;
//using Newtonsoft.Json.Linq;

namespace Laiye.Customer.WebApi.CAP
{
    public interface ICapSubscribeService
    {        
        /// 流程新增            
        public  Task FlowAdd(CommonEventArgs args);
        ///流程修改
        public Task FlowModify(CommonEventArgs args);
        //流程删除
        public Task FlowDelete(CommonEventArgs args);
        ///任务新增
        public Task TaskAdd(CommonEventArgs args);        
        public Task TaskSuccess(CommonEventArgs args);
        public Task TaskStop(CommonEventArgs args);
        ///worker创建
        public Task WorkerAdd(CommonEventArgs args);
        ///worker更新
        public Task WorkerUpdate(CommonEventArgs args);
        ///worker删除
        public Task WorkerDelete(CommonEventArgs args);
    }


    public class CapSubscribeService : ICapSubscribeService, ICapSubscribe
    {
        private ILogger Logger { get; }       
        public FlowClient FlowClient { get; }
        public TenantClient TenantClient { get; }
        public EmployeeClient EmployeeClient { get; }
        public WorkerClient WorkerClient { get; }
        public LicenseClient LicenseClient { get; }
        public TaskClient TaskClient { get; }
        public UserClient UserClient { get; }
        private IConfiguration Configuration { get; }
        public IFreeSql conn { get; }

      

        public CapSubscribeService(ILogger<CapSubscribeService> logger, FlowClient flowClient, TenantClient tenantClient, EmployeeClient employeeClient, 
            TaskClient taskClient, WorkerClient workclient, LicenseClient licenseclient, UserClient userClient,IFreeSql _conn, IConfiguration configuration)
        {
            Logger = logger;
            FlowClient = flowClient;
            TenantClient=tenantClient;
            EmployeeClient = employeeClient;
            TaskClient = taskClient;
            WorkerClient = workclient;
            LicenseClient = licenseclient;
            UserClient = userClient; 
            conn = _conn;
            Configuration = configuration;
        }

        /// <summary>
        /// 根据招商云的租户ID查找关系表数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        private async Task<RelationshipModel> QueryTenantInfo(IFreeSql conn, long companyid)
        {

            var Query = await conn.Select<RelationshipModel>().Where(a => a.CompanyId == companyid).ToListAsync();//.ToList();//Where(a=>a.CompanyId ==companyid);
            if (Query.Count == 0)
                return null;
            return Query.First();
        }

        public async Task<long> GetFlowRelationById(long flowId,  IFreeSql conn)
        {
            //_logger.LogInformation($"GetFlowRelationById <<1>> begin flowId:{flowId}");

            var flowRelationModel = await conn.Select<FlowRelationModel>()
                                            .Where(a => a.Flow_id == flowId).FirstAsync();

            if (flowRelationModel != null)
            {
              //  _logger.LogInformation($"GetFlowRelationById <<2>>: end Origin_flow_id:{flowRelationModel.Origin_flow_id}");
                return flowRelationModel.Origin_flow_id;
            }

            return 0;
        }



        //
        public async Task<string>  DoFlowInfo(int status, CommonEventArgs args)
        {

            Logger.LogInformation($"doFlowInfo 《1.{status}》  doFlowInfo begin");
            Logger.LogInformation($"doFlowInfo 《1.{status}》  GetFlowByIdAsync");
           
            var flow_resp= new EntCmd.Service.Core.GetFlowByIdResponse();

             if (status == 3)
             {
                    var flow_resp1 = await FlowClient.GetDeletedFlowByIdAsync(new EntCmd.Service.Core.GetFlowByIdRequest
                    {
                        CompanyId = args.CompanyId,
                        FlowId = args.Id
                    });
                    flow_resp=flow_resp1;
             }
            else
            {
                    var  flow_resp2 = await FlowClient.GetFlowByIdAsync(new EntCmd.Service.Core.GetFlowByIdRequest
                    {
                        CompanyId = args.CompanyId,
                        FlowId = args.Id
                    });
                     flow_resp = flow_resp2;
            }           

            //获取信息
            Logger.LogInformation($"doFlowInfo 《1.{status}》  flow_resp:{flow_resp}");
            long Id = flow_resp.Flow.Id;//流程ID
            Logger.LogInformation($"doFlowInfo 《1.{status}》  id:{Id}");
            string FlowName = flow_resp.Flow.FlowName;//流程名
            
            Logger.LogInformation($"doFlowInfo 《1.{status}》  FlowName:{FlowName}");
            long employeeid = status == 1 ? flow_resp.Flow.CreateEmployeeId : flow_resp.Flow.UpdateEmployeeId;
            Logger.LogInformation($"doFlowInfo 《1.{status}》  employeeid:{employeeid}");
            Timestamp operatetime = status == 1 ? flow_resp.Flow.CreateTime : flow_resp.Flow.UpdateTime;
            Logger.LogInformation($"doFlowInfo 《1.{status}》  operatetime:{operatetime}");

            if (employeeid == 0)
            {
                employeeid = flow_resp.Flow.CreateEmployeeId;
                Logger.LogInformation($"doFlowInfo 《1.{status}》  employeeid=0:{employeeid}");

            }

            if (operatetime == null)
            {
                operatetime = DateTime.UtcNow.ToTimestamp();
                Logger.LogInformation($"doFlowInfo 《1.{status}》  operatetime=null:{operatetime}");

            }


            Logger.LogInformation($"doFlowInfo 《1.{status}》  Id :{Id},FlowName:{FlowName},employeeid:{employeeid },operatetime:{operatetime}");            
            var employee_resp = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
            {
                EmployeeId= employeeid,
                CompanyId = args.CompanyId
            });


            Logger.LogInformation($"doFlowInfo 《1.{status}》  GetEmployeeByUserId Info:{employee_resp}");
            //获取姓名
            string username = employee_resp.Employee.UserName;

            var user_resp=  UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest 
            {
                UserName= username
            });
            //获取nickname参数
            username = user_resp.User.SourceValue;


            Logger.LogInformation($"doFlowInfo 《1.{status}》  username:{username}");

            //查询方法           
            var relationInfo_resp =await QueryTenantInfo(conn, args.CompanyId);

            Logger.LogInformation($"doFlowInfo 《1.{status}》  relationInfo_resp:{relationInfo_resp}");

            //Logger.LogInformation($"doFlowInfo 《1.{status}》  username:{username}");

            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            Logger.LogInformation($"doFlowInfo 《1.{status}》   TenantId_zsy:{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            Logger.LogInformation($"doFlowInfo 《1.{status}》   Projectid:{ Projectid}, flow_id:{Id}");
            string strContent = "";
            // flow_resp.Flow.SavingCost
            //flow_resp.Flow.SavingHour;
            //增加流程ID和源ID
            long orginid=await  GetFlowRelationById(Id, conn);
            Logger.LogInformation($"doFlowInfo 《1.{status}》 GetFlowRelationById orginid:{orginid}");
            switch (status)
            {
                case 1:
                    {

                        if (orginid == 0)
                        {
                            strContent = JsonConvert.SerializeObject(new ReportAddFlowInfo
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                //flow_resp.Flow.SavingHour
                                //flow_resp.Flow.SavingCost
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.SavingHour.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                creatorCode = username,
                                createTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });


                        }
                        else
                        {
                            strContent = JsonConvert.SerializeObject(new ReportAddFlowInfo2
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                //flow_resp.Flow.SavingHour
                                //flow_resp.Flow.SavingCost
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.SavingHour.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                originalFlowId= orginid.ToString(),
                                creatorCode = username,
                                createTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });

                        }


                        
                        Logger.LogInformation($"doFlowInfo 《1.{status}》   case1:{ strContent}");
                        break;
                    }
                case 2:
                    {
                        var flow_creator = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
                        {
                            EmployeeId = flow_resp.Flow.CreateEmployeeId,
                            CompanyId = args.CompanyId
                        });

                        var user_resp1 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = flow_creator.Employee.UserName
                        });
                        //获取nickname参数
                        string  creatorCode = user_resp1.User.SourceValue;
                        
                        if (orginid == 0)
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                               
                                creatorCode = creatorCode,// flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });

                        }
                        else
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo2
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                originalFlowId = orginid.ToString(),
                                creatorCode = creatorCode,// flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });

                        }

                           
                        Logger.LogInformation($"doFlowInfo 《1.{status}》   case2:{ strContent}");
                        break;
                    }
                case 3:
                    {
                        var flow_creator = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
                        {
                            EmployeeId = flow_resp.Flow.CreateEmployeeId,
                            CompanyId = args.CompanyId
                        });

                        var user_resp1 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = flow_creator.Employee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp1.User.SourceValue;

                        if (orginid == 0) 
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.SavingHour.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                creatorCode = creatorCode,//flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()

                            });

                        }
                        else
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo2
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.SavingHour.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                originalFlowId = orginid.ToString(),
                                creatorCode = creatorCode,//flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()

                            });

                        }



                          
                        Logger.LogInformation($"doFlowInfo 《1.{status}》   case3:{ strContent}");
                        break;
                    }
            }
                


            Logger.LogInformation($"doFlowInfo 《1.{status}》  strContent:{strContent}");
            return strContent;

        }


        public async Task<string> DoFlowInfo2(int status, CommonEventArgs args)
        {

            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  doFlowInfo begin");
            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  GetFlowByIdAsync");

            //var flow_resp = new EntCmd.Service.Core.GetFlowByIdResponse();

           
           
                var flow_resp = await FlowClient.GetFlowByIdAsync(new EntCmd.Service.Core.GetFlowByIdRequest
                {
                    CompanyId = args.CompanyId,
                    FlowId = args.Id
                });
          //      flow_resp = flow_resp2;
           

            //获取信息
            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  flow_resp:{flow_resp}");
            long Id = flow_resp.Flow.Id;//流程ID
            Logger.LogInformation($"zhaoshangyun..doFlowInfo 《1.{status}》  id:{Id}");
            string FlowName = flow_resp.Flow.FlowName;//流程名

            Logger.LogInformation($"zhaoshangyun..doFlowInfo 《1.{status}》  FlowName:{FlowName}");
            long employeeid = status == 1 ? flow_resp.Flow.CreateEmployeeId : flow_resp.Flow.UpdateEmployeeId;
            Logger.LogInformation($"zhaoshangyun..doFlowInfo 《1.{status}》  employeeid:{employeeid}");
            Timestamp operatetime = status == 1 ? flow_resp.Flow.CreateTime : flow_resp.Flow.UpdateTime;
            Logger.LogInformation($"zhaoshangyun..doFlowInfo 《1.{status}》  operatetime:{operatetime}");

            if (employeeid == 0)
            {
                employeeid = flow_resp.Flow.CreateEmployeeId;
                Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  employeeid=0:{employeeid}");

            }

            if (operatetime == null)
            {
                operatetime = DateTime.UtcNow.ToTimestamp();
                Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  operatetime=null:{operatetime}");

            }


            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  Id :{Id},FlowName:{FlowName},employeeid:{employeeid },operatetime:{operatetime}");
            var employee_resp = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
            {
                EmployeeId = employeeid,
                CompanyId = args.CompanyId
            });


            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  GetEmployeeByUserId Info:{employee_resp}");
            //获取姓名
            string username = employee_resp.Employee.UserName;

            var user_resp = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp.User.SourceValue;


            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  username:{username}");

            //查询方法           
            var relationInfo_resp = await QueryTenantInfo(conn, args.CompanyId);

            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  relationInfo_resp:{relationInfo_resp}");

            //Logger.LogInformation($"doFlowInfo 《1.{status}》  username:{username}");

            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》   TenantId_zsy:{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》   Projectid:{ Projectid}, flow_id:{Id}");
            string strContent = "";
            // flow_resp.Flow.SavingCost
            //flow_resp.Flow.SavingHour;
            //增加流程ID和源ID
            long orginid = await GetFlowRelationById(Id, conn);
            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》 GetFlowRelationById orginid:{orginid}");
            switch (status)
            {
               
                case 2:
                    {
                        var flow_creator = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
                        {
                            EmployeeId = flow_resp.Flow.CreateEmployeeId,
                            CompanyId = args.CompanyId
                        });

                        var user_resp1 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = flow_creator.Employee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp1.User.SourceValue;

                        if (orginid == 0)
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo3
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                shareType=1,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                creatorCode = creatorCode,// flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });

                        }
                        else
                        {
                            strContent = JsonConvert.SerializeObject(new ReportModifyFlowInfo4
                            {
                                id = Convert.ToString(Id),
                                tenantId = TenantId_zsy,
                                projectId = Projectid,
                                name = FlowName,
                                shareType = 1,
                                laborCost = (string.IsNullOrEmpty(flow_resp.Flow.ToString())) ? 1.0 : flow_resp.Flow.SavingHour,// 1.0暂时空缺
                                flowValue = (string.IsNullOrEmpty(flow_resp.Flow.SavingCost.ToString())) ? 100 : flow_resp.Flow.SavingCost,//   100暂时空缺
                                strategy = "按需调度",
                                url = "",
                                originalFlowId = orginid.ToString(),
                                creatorCode = creatorCode,// flow_creator.Employee.UserName,
                                createTime = flow_resp.Flow.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                updatorCode = username,
                                updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                            });

                        }

                        Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》   case2:{ strContent}");
                        break;
                    }
               




                       
            }



            Logger.LogInformation($"zhaoshangyun.doFlowInfo 《1.{status}》  strContent:{strContent}");
            return strContent;

        }



        public async Task<string> DoTaskInfo2(int status, long companyid,long id)
        {
            //0: add      1: success      3: stopping      2: delete
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  doTaskInfo begin");
            var task_resp = new EntCmd.Service.Core.GetTaskByIdResponse();
            if (status == 2)
            {
                var task_resp1 = await TaskClient.GetDeletedTaskByIdAsync(new EntCmd.Service.Core.GetTaskByIdRequest
                {
                    CompanyId = companyid,
                    Id =id
                });
                task_resp = task_resp1;
                Logger.LogInformation($"doTaskInfo( 《2.{status}》  delete Info");

            }
            else
            {
                var task_resp2 = await TaskClient.GetTaskByIdAsync(new EntCmd.Service.Core.GetTaskByIdRequest
                {
                    CompanyId = companyid,
                    Id = id
                });
                task_resp = task_resp2;
                Logger.LogInformation($"doTaskInfo( 《2.{status}》  QueryInfo");
            }

           

            Logger.LogInformation($"doTaskInfo( 《2.{status}》  GetTaskByIdAsync task_resp:{task_resp}");
            //获取信息
            long Id = task_resp.Task.Id;//任务ID
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  Id:{task_resp}");
            string FlowName = task_resp.Task.Flow.FlowName;//任务名（只能用流程名）
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  FlowName:{FlowName}");
            long FlowId = task_resp.Task.Flow.Id;//流程编号
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  FlowId:{FlowId}");                                      //

            //创建用户
            string username = task_resp.Task.TaskSimpleInfo.CreateUserName;

            var user_resp1 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp1.User.SourceValue;



            Logger.LogInformation($"doTaskInfo( 《2.{status}》 username:{username}");
            Timestamp operatetime = status == 0 ? task_resp.Task.TaskSimpleInfo.CreateTime : DateTime.UtcNow.ToTimestamp();
            Logger.LogInformation($"doTaskInfo( 《2.{status}》 operatetime:{operatetime}");

            //查询方法
            var relationInfo_resp = await QueryTenantInfo(conn, companyid);
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  QueryTenantInfo:" + relationInfo_resp);
            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  TenantId_zsy:{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  Projectid:{ Projectid}");
            // var tmpTimestamp = DateTime.UtcNow;

            string strContent = "";

            switch (status)
            {
                case 0:  //add
                    {
                        strContent = JsonConvert.SerializeObject(new ReportAddTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            status=-1,//修改
                            creatorCode = username,
                            createTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case0:{ strContent}");
                        break;
                    }
                case 1:   //success  shangbao
                    {
                        double db = (float)(task_resp.Task.TaskSimpleInfo.RunningTime / 3600.00);            //耗时 须转成小时
                        int Reportsubstate=1;
                        int SubState = (int)task_resp.Task.TaskRunningInfo.SubState;
                        
                        //1 任务成功
                        //SubState_Success = 40;
                        // 失败
                        //SubState_Failed = 50;
                        // 任务取消
                        //SubState_Canceled = 60;
                        // 任务停止
                        //SubState_Stoped = 70;
                        if (SubState == 40) { Reportsubstate = 1; };
                        if (SubState == 50) { Reportsubstate = 2; };
                        if (SubState == 60||SubState == 70) { Reportsubstate = 3; };

                        strContent = JsonConvert.SerializeObject(new ReportSuccessTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            //缺少startTime字段
                            startTime = task_resp.Task.TaskSimpleInfo.StartTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                                                       
                            endTime = task_resp.Task.TaskSimpleInfo.StopTime != null ? task_resp.Task.TaskSimpleInfo.StopTime.ToDateTimeOffset().ToUnixTimeMilliseconds():
                                                                                       task_resp.Task.TaskSimpleInfo.StartTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            robotId = task_resp.Task.TaskSimpleInfo.WorkerId,
                            duration = (float)Math.Round(db, 2),
                            status = Reportsubstate   //1-成功，3-取消
                                               //updatorCode = username,
                                               //updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),



                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case1:{ strContent}");
                        break;
                    }
                case 2:
                    {
                        var user_resp2 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = task_resp.Task.CreateEmployee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp2.User.SourceValue;


                        strContent = JsonConvert.SerializeObject(new ReportDeleteTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            status = status,   //1-成功，3-取消
                            updatorCode = username,
                            updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            creatorCode = creatorCode,//task_resp.Task.CreateEmployee.UserName,
                            createTime = task_resp.Task.TaskSimpleInfo.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case2:{ strContent}");

                        break;
                    }
                case 3:
                    {

                        var user_resp2 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = task_resp.Task.CreateEmployee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp2.User.SourceValue;


                        strContent = JsonConvert.SerializeObject(new ReportDeleteTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            status = status,
                            creatorCode = creatorCode,//task_resp.Task.CreateEmployee.UserName,
                            createTime = task_resp.Task.TaskSimpleInfo.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            updatorCode = username,
                            updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),

                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case3:{ strContent}");
                        break;
                    }
            }


            Logger.LogInformation($"doTaskInfo( 《2.{status}》 strContent:" + strContent);

            return strContent;

        }




        public async Task<string> DoTaskInfo(int status, CommonEventArgs args)
        {
            //0: add      1: success      3: stopping      2: delete
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  doTaskInfo begin");
            var task_resp = new EntCmd.Service.Core.GetTaskByIdResponse();
            if (status == 2)
            {
                var task_resp1 = await TaskClient.GetDeletedTaskByIdAsync(new EntCmd.Service.Core.GetTaskByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                task_resp = task_resp1;
                Logger.LogInformation($"doTaskInfo( 《2.{status}》  delete Info");

            }
            else
            {
                var task_resp2 = await TaskClient.GetTaskByIdAsync(new EntCmd.Service.Core.GetTaskByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                task_resp = task_resp2;
                Logger.LogInformation($"doTaskInfo( 《2.{status}》  QueryInfo");
            }
           
            
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  GetTaskByIdAsync task_resp:{task_resp}" );
            //获取信息
            long Id = task_resp.Task.Id;//任务ID
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  Id:{task_resp}" );
            string FlowName = task_resp.Task.Flow.FlowName;//任务名（只能用流程名）
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  FlowName:{FlowName}");
            long FlowId = task_resp.Task.Flow.Id;//流程编号
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  FlowId:{FlowId}");                                      //
            //addedbywenz 通过username查询nickname
            //创建用户
            string username  = task_resp.Task.TaskSimpleInfo.CreateUserName ;
            var user_resp1 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp1.User.SourceValue;



            Logger.LogInformation($"doTaskInfo( 《2.{status}》 username:{username}");
            Timestamp operatetime = status == 0 ? task_resp.Task.TaskSimpleInfo.CreateTime : DateTime.UtcNow.ToTimestamp();
            Logger.LogInformation($"doTaskInfo( 《2.{status}》 operatetime:{operatetime}");

            //查询方法
            var relationInfo_resp = await QueryTenantInfo(conn, args.CompanyId);
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  QueryTenantInfo:" + relationInfo_resp);
            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  TenantId_zsy:{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            Logger.LogInformation($"doTaskInfo( 《2.{status}》  Projectid:{ Projectid}");
            // var tmpTimestamp = DateTime.UtcNow;

            string strContent = "";

            switch (status)
            {
                case 0:  //add
                    {
                        strContent = JsonConvert.SerializeObject(new ReportAddTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            creatorCode = username,
                            createTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case0:{ strContent}");
                        break;
                    }
                case 1:   //success  shangbao
                    {
                        double db =(float) (task_resp.Task.TaskSimpleInfo.RunningTime / 3600.00);            //耗时 须转成小时

                        strContent = JsonConvert.SerializeObject(new ReportSuccessTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            //缺少startTime字段
                            startTime = task_resp.Task.TaskSimpleInfo.StartTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            robotId = task_resp.Task.TaskSimpleInfo.WorkerId,
                            duration = (float)Math.Round(db, 2),
                            status = status,   //1-成功，3-取消
                                               //updatorCode = username,
                                               //updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),



                        });;
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case1:{ strContent}");
                        break;
                    }
                case 2:
                    {
                        //新增username转成nickname
                        var user_resp2 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = task_resp.Task.CreateEmployee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp1.User.SourceValue;

                        strContent = JsonConvert.SerializeObject(new ReportDeleteTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            status = status,   //1-成功，3-取消
                            updatorCode = username,
                            updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            creatorCode = creatorCode, //task_resp.Task.CreateEmployee.UserName,
                            createTime = task_resp.Task.TaskSimpleInfo.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds()
                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case2:{ strContent}");

                        break;
                    }
                case 3:
                    {
                        var user_resp2 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
                        {
                            UserName = task_resp.Task.CreateEmployee.UserName
                        });
                        //获取nickname参数
                        string creatorCode = user_resp1.User.SourceValue;
                        strContent = JsonConvert.SerializeObject(new ReportDeleteTaskInfo
                        {
                            id = Convert.ToString(Id),
                            tenantId = TenantId_zsy,
                            projectId = Projectid,
                            flowId = FlowId.ToString(),
                            name = FlowName,
                            status = status,
                            creatorCode = creatorCode,//task_resp.Task.CreateEmployee.UserName,
                            createTime = task_resp.Task.TaskSimpleInfo.CreateTime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            updatorCode = username,
                            updateTime = operatetime.ToDateTimeOffset().ToUnixTimeMilliseconds(),
                            
                        });
                        Logger.LogInformation($"doTaskInfo( 《2.{status}》  case3:{ strContent}");
                        break;
                    }
            }

                
            Logger.LogInformation($"doTaskInfo( 《2.{status}》 strContent:" + strContent);

            return strContent;

        }

        public async Task<string> DoWorkerInfo(int status, CommonEventArgs args)
        {
            //status = 1:  add;     2: update;     3: delete    4: 状态上报
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  GetWorkerByIdAsync begin");

            var worker_resp= new EntCmd.Service.Core.GetWorkerByIdResponse();

            if (status == 3)
            {
                var worker_resp1 = await WorkerClient.GetDeletedWorkerByIdAsync(new EntCmd.Service.Core.GetWorkerByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                worker_resp = worker_resp1;
                Logger.LogInformation($"doWorkerInfo 《3.{status}》  delete");

            }
            else
            {
                var worker_resp2 = await WorkerClient.GetWorkerByIdAsync(new EntCmd.Service.Core.GetWorkerByIdRequest
                {
                    CompanyId = args.CompanyId,
                    Id = args.Id
                });
                worker_resp = worker_resp2;
                Logger.LogInformation($"doWorkerInfo 《3.{status}》  QueryInfo");

            }           
           
            Logger.LogInformation($"doWorkerInfo 《3.{status}》 GetWorkerByIdAsync{worker_resp}");
            long id = worker_resp.Worker.Id; //workerid
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  id:{id}");
            string workname = worker_resp.Worker.WorkerName;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  workname:{workname}");
            string ip = worker_resp.Worker.LastLoginIp;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  ip:{ip}");

            var workinfo_resp =await QueryWorkSubscripInfo(conn, args.CompanyId);

            var tmpTimestamp = DateTime.UtcNow; //- DateTime.Now;
            string license = workinfo_resp == null ? "无授权" : "浮动授权";
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  license:{license}");
            DateTime ls_begintime = workinfo_resp != null ? workinfo_resp.Ls_begintime : tmpTimestamp;//License 开始时间
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  ls_begintime:{ls_begintime}");
            DateTime Ls_endtime = workinfo_resp != null ? workinfo_resp.Ls_endtime : tmpTimestamp;//License结束时间
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  Ls_endtime:{Ls_endtime}");
            string OrderNo_zsy = workinfo_resp.OrderNo_zsy;//worker订单号
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  OrderNo_zsy:{OrderNo_zsy}");
            int workertype = worker_resp.Worker.IsAccept;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  workertype:{ workertype}");
            int workerstatus = worker_resp.Worker.IsOnline;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  workerstatus:{workerstatus}");

            //要记得修改ent_cmd service的领域参数，现在默认带了
            WorkerUpdatedEventArgs extArgs = null;           

            if (status != 1&&status != 4)
            {
                Logger.LogInformation($"doWorkerInfo 《3.{status}》  rstatus 2,3:{status}");
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
                        Logger.LogInformation($"doWorkerInfo 《3.{status}》  case1");
                        break;
                }
                case 2:
                {
                        operatetime = extArgs.updateTime.ToTimestamp();
                        employee_id = extArgs.updateEmployeeId;
                        Logger.LogInformation($"doWorkerInfo 《3.{status}》  case2");
                        break;
                 }
                case 3:
                {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;
                        Logger.LogInformation($"doWorkerInfo 《3.{status}》  case3");
                        break;
                }
                case 4:
                    {
                        operatetime = DateTime.UtcNow.ToTimestamp();
                        employee_id = worker_resp.Worker.CreateEmployeeId;
                        
                        Logger.LogInformation($"doWorkerInfo 《3.{status}》  case4");
                        break;
                    }
            }             

            var employee_resp = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
            {
                EmployeeId = employee_id,
                CompanyId = args.CompanyId//
            });
            
            
            string username = employee_resp.Employee.UserName;


            var user_resp2 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
            {
                UserName = username
            });
            //获取nickname参数
            username = user_resp2.User.SourceValue;


            Logger.LogInformation($"doWorkerInfo 《3.{status}》  username{ username}");
            Logger.LogInformation($"doWorkerInfo 《3.{status}》 GetEmployeeByUserId{employee_resp}");

                        
            //查询方法
            var relationInfo_resp = await QueryTenantInfo(conn, args.CompanyId);
            Logger.LogInformation($"doWorkerInfo 《3.{status}》QueryTenantInfo{relationInfo_resp}");

            string TenantId_zsy = relationInfo_resp.TenantId_zsy;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  TenantId_zsy{ TenantId_zsy}");
            string Projectid = relationInfo_resp.Projectid;
            Logger.LogInformation($"doWorkerInfo 《3.{status}》  Projectid{Projectid}");
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
                    createTime = operatetime.ToDateTimeOffset().ToUnixTimeSeconds()*1000
                    
                }) ;

            if (status == 2 || status == 3) {

                var employee_resp1 = EmployeeClient.GetEmployeeById(new EntUC.Service.Tenant.GetEmployeeByIdRequest
                {
                    EmployeeId = worker_resp.Worker.CreateEmployeeId,
                    CompanyId = args.CompanyId//
                });


                var user_resp3 = UserClient.GetUserByUserName(new EntUC.Service.User.GetUserByUserNameRequest
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
                    creatorCode = creatorCode,//worker_resp.Worker.CreateEmployee,
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
                }) ;
                Logger.LogInformation($"doWorkerInfo 《3.{status}》  status {status}");
            }

            Logger.LogInformation($"doWorkerInfo 《3.{status}》strContent{strContent}");
            return strContent;
        }

        //查询订单返回
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

        private class WorkerUpdatedEventArgs
        {
           public long NewMachineId { get; set; }

            public long OldMachineId { get; set; }

            public bool NeedReboot { get; set; }

            public DateTime updateTime { get; set; }
    
            public long updateEmployeeId { get; set; }



        }

        public class TaskCompletedEventArgsItem
        {
            /// <summary>
            /// 租户Id
            /// </summary>
            public long CompanyId { get; set; }

            /// <summary>
            /// 任务Id
            public ESubState SubState { get; set; }
            /// </summary>
            public long TaskId { get; set; }

            /// <summary>
            /// WorkerId
            /// </summary>
            public long WorkerId { get; set; }
        }

        private class TaskCompletedEventArgs
        {
            public TaskCompletedEventArgsItem[] Items { get; set; }       
        }
        /**
         *  CompanyId = request.CompanyId,
                EventArgs = JsonConvert.SerializeObject(new
                {
                    CompanyId = request.CompanyId,
                    TriggerRecordId = request.Trigger?.TriggerRecordId ?? 0,
                    TaskIds = result,
                    WorkerGroupId = request.UnattendedTaskConfiguration?.WorkerGroupInfo?.Id ?? 0,
                    WorkerId = request.UnattendedTaskConfiguration?.WorkerInfo?.Id ?? 0,
                    TriggerId = request.Trigger?.Id ?? 0
                })
         * 
         * 
         */

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public long  CompanyId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public EventArgs EventArgs { get; set; }

        }

        public class EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            public long  CompanyId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long TriggerRecordId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long[] TaskIds { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long WorkerGroupId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long WorkerId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long TriggerId { get; set; }

        }






        //待添加
        [CapSubscribe("cmd.flow.created")]
        public async Task FlowAdd( CommonEventArgs args)
        {
            Logger.LogInformation("FlowAdd 《1》  doFlowInfo begin");

            string requestinfo =await  DoFlowInfo(1, args);

            Logger.LogInformation("FlowAdd 《1》 doFlowInfo end");
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey=Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["flow_info"],
                // CommanderUrl = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api"
            }, 2);


            Logger.LogInformation($"FlowAdd 《1》 ReportPostInfo :{requestinfo}");

            var ret= client.reportInfoClient.ReportPostInfo(requestinfo);

            Logger.LogInformation($"FlowAdd 《1》 ReportPostInfo Ret:{ret}");
            if (ret == false) { Logger.LogInformation("FlowAdd 《1》 ReportPostInfo Error"); };
        }


        [CapSubscribe("cmd.flow.updated")]
        public async Task FlowModify(CommonEventArgs args)
        {
            Logger.LogInformation("FlowModify《1》 doFlowInfo begin");

            string requestinfo = await DoFlowInfo(2,args);

            Logger.LogInformation("FlowModify《1》 doFlowInfo end");

            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["flow_info"],
               
            }, 2);
            Logger.LogInformation($"FlowModify 《1》 ReportPostInfo :{requestinfo}");

            var ret = client.reportInfoClient.ReportPutInfo(requestinfo);
            //var ret = await client.reportInfoClient.ReportPostInfo(requestinfo);

            Logger.LogInformation($"FlowModify《1》 ReportPutInfo ret{ret}");
            if (ret == false) { Logger.LogInformation("FlowModify 《1》 ReportPostInfo Error"); };


            
        }
        //增加一项将传进来的流程用于更新
        [CapSubscribe("zsy.flow.updated")]
        public async Task FlowModifyBySelf(CommonEventArgs args)
        {
            Logger.LogInformation("zhaoshangyu.flow《1》 doFlowInfo begin");

            string requestinfo = await DoFlowInfo2(2, args);

            Logger.LogInformation("zhaoshangyu.flow《1》 doFlowInfo end");

            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["flow_info"],

            }, 2);
            Logger.LogInformation($"zhaoshangyu.flow 《1》 ReportPostInfo :{requestinfo}");

            var ret = client.reportInfoClient.ReportPutInfo(requestinfo);
            //var ret = await client.reportInfoClient.ReportPostInfo(requestinfo);

            Logger.LogInformation($"zhaoshangyu.flow《1》 ReportPutInfo ret{ret}");
            if (ret == false) { Logger.LogInformation("zhaoshangyu.flow 《1》 ReportPostInfo Error"); };



        }






        [CapSubscribe("cmd.flow.deleted")]
        public async Task FlowDelete(CommonEventArgs args)
        {
            Logger.LogInformation("FlowDelete《1》 doFlowInfo begin");

            string requestinfo = await DoFlowInfo(3, args);
            Logger.LogInformation("FlowDelete《1》 doFlowInfo end");

            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],                   //取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                   
                CommanderUrl = Configuration["flow_info"],
            }, 2);

            Logger.LogInformation($"FlowDelete 《1》 ReportPostInfo :{requestinfo}");

            var ret = await client.reportInfoClient.ReportDeleteInfo(requestinfo);

            Logger.LogInformation($"FlowDelete 《1》 ReportPostInfo ret :{ret}");

            if (ret == false) { Logger.LogInformation("FlowDelete 《1》 ReportPostInfo Error"); };
        }

        //任务新增
        [CapSubscribe("cmd.task.created")]
        public async Task TaskAdd(CommonEventArgs args)
        
        {
            Logger.LogInformation("TaskAdd 《2》 doTaskInfo:begin ");
            //获取args
            var extArgs = JsonConvert.DeserializeObject<EventArgs>(args.EventArgs);

            for (int i = 0; i < extArgs.TaskIds.Length; i++)
            {
                long cid = extArgs.CompanyId;
                Logger.LogInformation($"TaskAdd 《2》 CompanyId:{cid }");
                long tid = extArgs.TaskIds[i];
                Logger.LogInformation($"TaskAdd 《2》 TaskId:{tid }");

                string requestinfo = await DoTaskInfo2(0, cid, tid);

                Logger.LogInformation($"TaskAdd 《2》doTaskInfo:{requestinfo}");
                var client = new CommanderClient(new CommanderClientOption
                {
                    XAuthProduct = Configuration["X-Auth-Product"],
                    XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                    CommanderUrl = Configuration["task_info"],

                }, 2);

                Logger.LogInformation($"TaskAdd 《2》 ReportPostInfo :{requestinfo}");

                Thread.Sleep(50);

                var ret = client.reportInfoClient.ReportPostInfo(requestinfo);

                Logger.LogInformation($"TaskAdd 《2》 ReportPostInfo ret :{ret}");
                if (ret == false) { Logger.LogInformation("TaskAdd 《2》 ReportPostInfo Error"); };
            }
            

                
            



            //string requestinfo = await doTaskInfo(0, args);

            //Logger.LogInformation($"TaskAdd 《2》doTaskInfo:{requestinfo}");
            //var client = new CommanderClient(new CommanderClientOption
            //{
            //    XAuthProduct = Configuration["X-Auth-Product"],//取文件
            //    XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
            //    CommanderUrl = Configuration["task_info"],

            //}, 2);
            //Logger.LogInformation($"TaskAdd 《2》 ReportPostInfo :{requestinfo}");

            //var ret = await client.reportInfoClient.ReportPostInfo(requestinfo);

            //Logger.LogInformation($"TaskAdd 《2》 ReportPostInfo ret :{ret}");
            //if (ret == false) { Logger.LogInformation("TaskAdd 《2》 ReportPostInfo Error"); };
           
        }
        //任务上报  这个接口要写task上报 job/state/
        [CapSubscribe("cmd.task.task.completed")]
        public async Task TaskSuccess(CommonEventArgs args)
        {
            Logger.LogInformation($"TaskSuccess 《2》 args:{ args.ToString()}");

            //args = new CommonEventArgs
            //{
            //    EventArgs = JsonConvert.SerializeObject(new
            //    {
            //        Items = new[]
            //               {
            //                    new
            //                    {
            //                        CompanyId = 50,
            //                        SubState = 10,
            //                        TaskId = 2895190914236417,
            //                        WorkerId = 2857568930103297
            //                    },
            //                     new
            //                    {
            //                        CompanyId = 50,
            //                        SubState = 10,
            //                        TaskId = 2895190914236419,
            //                        WorkerId = 2857568930103297
            //                    }
            //                }
            //    })
            //};



            Logger.LogInformation(" begin JsonConvert.DeserializeObject ");
            var  extArgsItems = JsonConvert.DeserializeObject<TaskCompletedEventArgs>(args.EventArgs);
            Logger.LogInformation(" end JsonConvert.DeserializeObject ");
                   
            foreach (TaskCompletedEventArgsItem item in extArgsItems.Items)
            {
                long cid =item.CompanyId;
                Logger.LogInformation($"TaskSuccess 《2》 companyid:{cid}");
                long tid = item.TaskId;
                Logger.LogInformation($"TaskSuccess 《2》 taskid:{tid}");
                string requestinfo = await DoTaskInfo2(1, cid, tid);
                Logger.LogInformation($"TaskSuccess 《2》 doTaskInfo:{requestinfo}");
                var client = new CommanderClient(new CommanderClientOption
                {
                    XAuthProduct = Configuration["X-Auth-Product"],
                    XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                    CommanderUrl = Configuration["task_info"] + "/state",//上报任路径

                }, 2);
                Logger.LogInformation($"TaskSuccess 《2》 ReportPostInfo:{requestinfo}");
                              
                var ret = client.reportInfoClient.ReportPostInfo(requestinfo);
                if (ret == false)
                {
                    Logger.LogInformation("TaskSuccess 《2》 ReportPostInfo Error"); 
                }
                Thread.Sleep(50);
            }


           //     string requestinfo = await doTaskInfo(1, args);

           // Logger.LogInformation($"TaskSuccess 《2》 doTaskInfo:{requestinfo}");
           // var client = new CommanderClient(new CommanderClientOption
           // {
           //     XAuthProduct = Configuration["X-Auth-Product"],
           //     XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
           //     CommanderUrl = Configuration["task_info"]+ "/state",//上报任路径

           // }, 2);
           // Logger.LogInformation($"TaskSuccess 《2》 ReportPostInfo:{requestinfo}");

           ////var ret = await client.reportInfoClient.ReportPutInfo(requestinfo);
           //   var ret = await client.reportInfoClient.ReportPostInfo(requestinfo);
           //   if (ret == false) { Logger.LogInformation("TaskSuccess 《2》 ReportPostInfo Error"); };


           
        }
        [CapSubscribe("cmd.task.stopping")]//任务变更接口走put方法
        public async Task TaskStop(CommonEventArgs args)
        {
            Logger.LogInformation("TaskStop 《2》 doTaskInfo:begin ");

            string requestinfo = await DoTaskInfo(3, args);

            Logger.LogInformation($"TaskStop 《2》 doTaskInfo:{requestinfo} ");
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["task_info"],
            }, 2);
            Logger.LogInformation($"TaskStop 《2》 ReportPutInfo:{requestinfo}");

            var ret = client.reportInfoClient.ReportPutInfo(requestinfo);
            //var ret = await client.reportInfoClient.ReportPostInfo(requestinfo);

            Logger.LogInformation($"TaskStop 《2》 ReportPutInfo ret:{ret}");

            if (ret == false) 
            { 
                Logger.LogInformation("TaskStop 《2》 ReportPostInfo Error"); 
            };
           
        }

         

        [CapSubscribe("cmd.worker.created")]
        public async Task WorkerAdd(CommonEventArgs args)
        {
            Logger.LogInformation("WorkerAdd 《3》 doWorkInfo:begin ");
            string requestinfo = await DoWorkerInfo(1, args);
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["worker_info"],

            }, 2);

            Logger.LogInformation($"WorkerAdd     ReportPostInfo   requestioninfo is :  {requestinfo}");
            var ret = client.reportInfoClient.ReportPostInfo(requestinfo);
            if (ret == false)
            {
                Logger.LogInformation("client.reportInfoClient.ReportPostInfo failed");
                //throw new NotImplementedException(); 
            };            
        }

        [CapSubscribe("cmd.worker.status")]
        public async Task WorkerStatus(CommonEventArgs args)
        {
            Logger.LogInformation("WorkerAdd 《4》 doWorkInfo:begin ");
            string requestinfo = await DoWorkerInfo(4, args);
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["worker_info"] + "/state",//走上报的state路径

            }, 2);

            Logger.LogInformation($"WorkerAdd     ReportPostInfo   requestioninfo is :  {requestinfo}");
            var ret = client.reportInfoClient.ReportPostInfo(requestinfo);
            if (ret == false)
            {
                Logger.LogInformation("client.reportInfoClient.ReportPostInfo failed");
                //throw new NotImplementedException(); 
            };
        }

        [CapSubscribe("cmd.worker.updated")]
                       
        public async Task WorkerUpdate(CommonEventArgs args)
        {
           
            string requestinfo = await DoWorkerInfo(2, args);
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["worker_info"],

            }, 2);

            Logger.LogInformation($"Workerupdate  requestioninfo is :  {requestinfo}");
            var ret = client.reportInfoClient.ReportPutInfo(requestinfo);
            if (ret == false)
            {
              Logger.LogInformation("WorkerUpdate.ReportPostInfo failed");
            };
        }


        [CapSubscribe("cmd.worker.deleted")]
                       
        public async Task WorkerDelete(CommonEventArgs args)
        {
            string requestinfo =await DoWorkerInfo(3, args);
            var client = new CommanderClient(new CommanderClientOption
            {
                XAuthProduct = Configuration["X-Auth-Product"],//取文件
                XGatewayApikey = Configuration["X-Gateway-Apikey"],                                                     // HeadersKey = "78d0d642-a8bc-4f36-9565-d51c5086169c",
                CommanderUrl = Configuration["worker_info"],

            }, 2);

            Logger.LogInformation($"WorkerDelete     ReportPostInfo   requestioninfo is :  {requestinfo}");
            var ret = await client.reportInfoClient.ReportDeleteInfo(requestinfo);
            if (ret == false)
            {
               //throw new NotImplementedException();
                Logger.LogInformation("WorkerDelete  ReportDeleteInfo Error");
            };

        }

       

        

   

       


       


    }
}
