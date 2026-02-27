using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model.Internal.MarshallTransformations;
using com.sun.xml.@internal.ws.addressing.policy;
using Google.Protobuf.Reflection;
using java.lang;
using javax.swing;
using jdk.nashorn.@internal.ir;
using Laiye.Customer.WebApi.Model;
using Laiye.Customer.WebApi.Model.Dto;
using Laiye.EntUC.Core.Common;
using Laiye.EntUC.Service.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql.Logging;
using org.omg.SendingContext;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.Linq;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static FreeSql.Internal.GlobalFilter;
using static Laiye.Customer.WebApi.Controllers.ExtendBussinessController;
using Boolean = System.Boolean;
using Exception = System.Exception;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using StringBuilder = java.lang.StringBuilder;
using javax.management;

/**
 *  成果展示
 */
namespace Laiye.Customer.WebApi.Controllers
{
    [ApiController]
    [Route("huaxia/screen/dashboard")]
    public class HuaxiajijinController : ControllerBase
    {
        private ILogger<ExtendBussinessController> Logger { get; }
        private IConfiguration Configuration { get; }

        public HuaxiajijinController(ILogger<ExtendBussinessController> _logger, IConfiguration _configuration)
        {
            Logger = _logger;
            Configuration = _configuration;
        }

        [HttpPost("test")]
        public BaseResponse<string> Test([FromServices] IFreeSql conn)
        {
            return new BaseResponse<string>("Hello!");
        }

        /**
         *  验证用户名与密码
         */
        [HttpPost("verifyLogin")]
        public BaseResponse<string> verifyLogin([FromServices] IFreeSql conn, [FromBody] LoginParamer request)
        {
            var userLoginId = request.userLoginId;
            var password = request.password;
            var result = "notPass";
            if (!string.IsNullOrEmpty(userLoginId)) {
                StringBuilder sb = new StringBuilder();
                sb.append("  select user_login_id as userLoginId,current_password as currentPassword from HUAXIA.user_login ");
                sb.append(" where user_login_id = '" + userLoginId + "'");

                var sql = sb.toString();

                var base64Password  = getMd5(password,64);

                var userLogin = conn.Select<UserLoginBean>().WithSql(@sql).ToOne();
                if (userLogin != null) {
                    var currentPassword = userLogin.currentPassword;
                    if (base64Password.Equals(currentPassword)) {
                        result = "pass";
                        return new BaseResponse<string>(result);
                    }
                }
            }
            if (result.Equals("notPass")) {
                return new BaseResponse<string>(result, "803", "用户名密码不正确");
            }
            return new BaseResponse<string>(result);
        }

        /**
         *  新增登录用户
         */
        [HttpPost("addUserLogin")]
        public BaseResponse<string> addUserLogin([FromServices] IFreeSql conn, [FromBody] LoginParamer request)
        {
            var userLoginId = request.userLoginId;
            var password = request.password;            // 新密码

            var result = "";
            if (!string.IsNullOrEmpty(userLoginId))
            {
                StringBuilder sb = new StringBuilder();
                sb.append("  select user_login_id as userLoginId,current_password as currentPassword from HUAXIA.user_login ");
                sb.append(" where user_login_id = '" + userLoginId + "'");
                var sql = sb.toString();

                var userLogin = conn.Select<UserLoginBean>().WithSql(@sql).ToOne();
                if (userLogin != null)
                {
                    result = "用户名已存在！";
                    return new BaseResponse<string>(result, "803", "用户名已存在！");
                } else {
                    var base64Password = getMd5(password, 64);
                    conn.InsertOrUpdate<UserLogin>().SetSource(new UserLogin
                    {
                        userLoginId = request.userLoginId,
                        currentPassword = base64Password,
                    }).ExecuteAffrows();
                    result = "新增成功！";
                    return new BaseResponse<string>(result);
                }
            } else  {
                result = "用户名为空了！";
                return new BaseResponse<string>(result, "804", "用户名为空了！");
            }
            return new BaseResponse<string>(result);
        }


        /**
         *  修改用户密码
         */
        [HttpPost("updatePassword")]
        public BaseResponse<string> updatePassword([FromServices] IFreeSql conn, [FromBody] PasswordParamer request)
        {
            var userLoginId = request.userLoginId;
            var password = request.password;           // 新密码
            var oldPassword = request.oldPassword;     // 旧密码

            var result = "";
            if (!string.IsNullOrEmpty(userLoginId))
            {
                StringBuilder sb = new StringBuilder();
                sb.append("  select user_login_id as userLoginId,current_password as currentPassword from HUAXIA.user_login ");
                sb.append(" where user_login_id = '" + userLoginId + "'");

                var sql = sb.toString();

                var userLogin = conn.Select<UserLoginBean>().WithSql(@sql).ToOne();
                if (userLogin != null)
                {
                    var base64Password = getMd5(oldPassword, 64);

                    // 验证原来的密码是否正确
                    var currentPassword = userLogin.currentPassword;
                    if (base64Password.Equals(currentPassword))
                    {
                        var newBase64Password = getMd5(password, 64);
                        conn.InsertOrUpdate<UserLogin>().SetSource(new UserLogin
                        {
                            userLoginId = request.userLoginId,
                            currentPassword = newBase64Password,
                        }).ExecuteAffrows();
                        result = "修改成功！";
                        return new BaseResponse<string>(result);
                    } else {
                        result = "原密码不正确";
                        return new BaseResponse<string>(result,"801", "原密码不正确");
                    }
                } else {
                    result = "用户名不存在！";
                    return new BaseResponse<string>(result, "802", "用户名不存在！");
                }
            } else {
                result = "用户名不存在！";
                return new BaseResponse<string>(result, "802", "用户名不存在！");
            }
            return new BaseResponse<string>(result);
        }



        /**
         * 密码加密
         */
        public static string getMd5(string password, int codeLength)
        {
            string cl = password;
            MD5 md5 = MD5.Create(); // 实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }


        /**
         * 今日成功、失败、运行、待运行、成功率
         */
        [HttpPost("topinfoTodayRuninfo")]
        public BaseResponse<TodayRuninfoBean> TopinfoTodayRuninfo([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select today_tasksuccess as todayTasksuccess,today_taskfailed as todayTaskfailed,today_taskrunning as todayTaskrunning,today_taskdeploying as todayTaskdeploying from HUAXIA.t_dashboard_result_topinfo_today_runinfo ");
                sb1.append(" where update_date=DATE_FORMAT(NOW(),'%Y-%m-%d')  ");
                sb1.append(" and update_time=(select max(update_time) from HUAXIA.t_dashboard_result_topinfo_today_runinfo  ");
                sb1.append("  where update_date=DATE_FORMAT(NOW(),'%Y-%m-%d')) ");
                var sql1 = sb1.toString();
                var todayRuninfo = conn.Select<TodayRuninfo>().WithSql(@sql1).ToOne();

                long todayTasksuccess = todayRuninfo.todayTasksuccess;
                long todayTaskfailed = todayRuninfo.todayTaskfailed;

                // 成功率
                float taskSuccessRate = 0f;
                if (todayTasksuccess != 0 && todayTaskfailed != 0) {
                    float taskSuccessRateTemp = ((float)todayTasksuccess / (todayTasksuccess + todayTaskfailed)) * 100;
                    taskSuccessRate = float.Parse(taskSuccessRateTemp.ToString("#0.00"));
                } else if (todayTasksuccess > 0 && todayTaskfailed == 0){
                    taskSuccessRate = 100l;
                } else if (todayTasksuccess == 0 || todayTaskfailed == 0) {
                    taskSuccessRate = 0f;
                }

                var todayRuninfoBean = new TodayRuninfoBean();
                todayRuninfoBean.todayTasksuccess = todayTasksuccess;
                todayRuninfoBean.todayTaskfailed = todayTaskfailed;
                todayRuninfoBean.todayTaskrunning = todayRuninfo.todayTaskrunning;
                todayRuninfoBean.todayTaskdeploying = todayRuninfo.todayTaskdeploying;
                todayRuninfoBean.taskSuccessRate = taskSuccessRate;
                return new BaseResponse<TodayRuninfoBean> { data = todayRuninfoBean };
            }
            catch (Exception ex)
            {
                string message = "topinfoTodayRuninfo()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<TodayRuninfoBean> { };
            }
        }



        /**
         * 运行成功次数、流程总数、计划任务数、节省工时、流程部门数、用户数
         */
        [HttpPost("taskStatistics")]
        public BaseResponse<TaskFlowBean> TaskStatistics([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select flowCount,totalTask,taskSucceed,taskFailed,laborTime,taskSuccessRate,flow_deptcount as flowDeptcount,usercount from HUAXIA.t_dashboard_result_topinfo where update_date=date_format(NOW(),'%Y-%m-%d') ");
                var sql1 = sb1.toString();
                var taskFlowBean = conn.Select<TaskFlowBean>().WithSql(@sql1).ToOne();
                if (taskFlowBean == null) {
                    taskFlowBean = new TaskFlowBean
                    {
                        flowCount = 0,
                        totalTask = 0,
                        taskSucceed = 0,
                        taskFailed = 0,
                        flowDeptcount = 0,
                        usercount =0,
                        laborTime = 0,
                        taskSuccessRate = 0,
                    };
                }
                return new BaseResponse<TaskFlowBean> { data = taskFlowBean };
            }
            catch (Exception ex)
            {
                string message = "taskStatistics()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<TaskFlowBean> { };
            }
        }



        /**
         * 任务完成排行榜
         */
        [HttpPost("taskFinishDepartment")]
        public BaseResponse<List<TaskFinishBean>> TaskFinishDepartment([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_name as deptName, usernum as userNum,workernum as workerNum,tasknum as taskNum, ");
                sb1.append(" flownum as flowNum,savingWorkingHour as workHour from HUAXIA.t_dashboard_result_TaskFinish_Department where update_date=date_format(NOW(),'%Y-%m-%d')");
                var sql1 = sb1.toString();
                var query = conn.Select<TaskFinishBean>().WithSql(@sql1).ToList();

                var taskFinishList = new List<TaskFinishBean>();
                for (int j = 0; j < query.Count; j++)
                {
                    var deptName = query[j].deptName;
                    var workerNum = query[j].workerNum;
                    var userNum = query[j].userNum;
                    var taskNum = query[j].taskNum;
                    var flowNum = query[j].flowNum;
                    var workHour = query[j].workHour;

                    if (workerNum != 0 && userNum != 0 && taskNum != 0 && flowNum != 0 && workHour != 0) {
                        taskFinishList.Add(new TaskFinishBean
                        {
                            deptName = deptName,
                            workerNum = workerNum,
                            userNum = userNum,
                            taskNum = taskNum,
                            flowNum = flowNum,
                            workHour = workHour,
                        });
                    }
                }

                return new BaseResponse<List<TaskFinishBean>> { data = taskFinishList };
            }
            catch (Exception ex)
            {
                string message = "taskFinishDepartment()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);

                return new BaseResponse<List<TaskFinishBean>> { };
            }
        }

        /**
         * 人机交互在线与离线统计
         * 无人值守在线与离线统计
         */
        [HttpPost("workerOnline")]
        public BaseResponse<WorkerBean> WorkerOnline([FromServices] IFreeSql conn)
        {
            try
            {
                // 人机交互在线
                // 注意: 达梦数据库中tbl_cmd_attended_worker表使用IS_ONLINE字段而不是worker_state
                // IS_ONLINE=1 表示在线, IS_ONLINE=0 或 NULL 表示离线
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select count(*) as manMachineOnline  from RPA.tbl_cmd_attended_worker where IS_ONLINE = 1  ");
                var sql1 = sb1.toString();
                var manMachineBean = conn.Select<ManOnlineBean>().WithSql(@sql1).ToOne();

                // 人机交互离线
                StringBuilder sb2 = new StringBuilder();
                sb2.append(" select count(*) as manMachineNotOnline  from RPA.tbl_cmd_attended_worker where (IS_ONLINE = 0 OR IS_ONLINE IS NULL)  ");
                var sql2 = sb2.toString();
                var manNotOnlineBean = conn.Select<ManNotOnlineBean>().WithSql(@sql2).ToOne();

                // 无人值守在线
                // 注意: tbl_cmd_worker表使用IS_DELETED字段判断是否删除,worker_state判断在线状态
                StringBuilder sb3 = new StringBuilder();
                sb3.append(" select count(*) as unmannedOnline  from RPA.tbl_cmd_worker where (IS_DELETED = 0 OR IS_DELETED IS NULL) and worker_state=2 ");
                var sql3 = sb3.toString();
                var unManOnlineBean = conn.Select<UnManOnlineBean>().WithSql(@sql3).ToOne();

                // 无人值守离线
                StringBuilder sb4 = new StringBuilder();
                sb4.append(" select count(*) as unmannedNotOnline from RPA.tbl_cmd_worker where (IS_DELETED = 0 OR IS_DELETED IS NULL) and worker_state=3 ");
                var sql4 = sb4.toString();
                var unManNotOnlineBean = conn.Select<UnManNotOnlineBean>().WithSql(@sql4).ToOne();

                var workerBean = new WorkerBean
                {
                    manMachineOnline = manMachineBean.manMachineOnline,
                    manMachineNotOnline = manNotOnlineBean.manMachineNotOnline,
                    unmannedOnline = unManOnlineBean.unmannedOnline,
                    unmannedNotOnline = unManNotOnlineBean.unmannedNotOnline,
                };
                return new BaseResponse<WorkerBean>(workerBean);
            }
            catch (Exception ex)
            {
                string message = "workerOnline()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
            }
            return new BaseResponse<WorkerBean>();
        }

        /**
         * 部门执行数量、执行成功率
         */
        [HttpPost("taskSuccRateDepartment")]
        public BaseResponse<List<TaskSuccRateDepartmentBean>> TaskSuccRateDepartment([FromServices] IFreeSql conn)
        {
            try
            {
                var dlist = new List<TaskSuccRateDepartmentBean>();
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_id as depId,dep_name as deptName,totalTask,totalSucc,succRatePercent from HUAXIA.t_dashboard_result_TaskSuccRate_Department  where update_date=date_format(NOW(),'%Y-%m-%d')");
                var sql1 = sb1.toString();
                var query = conn.Select<TaskSuccRateDepartmentBean>().WithSql(@sql1).ToList();

                for (int j = 0; j < query.Count; j++)
                {
                    string depId = query[j].depId;
                    string deptName = query[j].deptName;
                    long totalTask = query[j].totalTask;
                    long totalSucc = query[j].totalSucc;
                    float succRatePercent = query[j].succRatePercent;

                    if (totalTask > 0 && totalSucc > 0) {
                        dlist.Add(new TaskSuccRateDepartmentBean
                        {
                            depId = depId,
                            deptName = deptName,
                            totalTask = totalTask,
                            totalSucc = totalSucc,
                            succRatePercent = succRatePercent,
                        });
                    }
                }
                return new BaseResponse<List<TaskSuccRateDepartmentBean>> { data = dlist };
            }
            catch (Exception ex)
            {
                string message = "taskSuccRateDepartment()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskSuccRateDepartmentBean>> { };
            }
        }



        /**
         * 节省工时Top10
         */
        [HttpPost("savingWorkHourDep")]
        public BaseResponse<List<SavingWorkHourDepBean>> SavingWorkHourDep([FromServices] IFreeSql conn)
        {
            try
            {
                var dlist = new List<SavingWorkHourDepBean>();
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_id as depId,depname as deptName,savingtime from HUAXIA.t_dashboard_result_SavingWorkHourDep_Top10 where update_date=date_format(NOW(),'%Y-%m-%d') ");
                var sql1 = sb1.toString();
                var query = conn.Select<SavingWorkHourDepBean>().WithSql(@sql1).ToList();

                for (int j = 0; j < query.Count; j++)
                {
                    string depId = query[j].depId;
                    string deptName = query[j].deptName;
                    float savingtime = query[j].savingtime;
                    string savingtimeRate = savingtime.ToString("#0.00");
                    dlist.Add(new SavingWorkHourDepBean
                    {
                        depId = depId,
                        deptName = deptName,
                        savingtime = float.Parse(savingtimeRate),
                    });
                }
                return new BaseResponse<List<SavingWorkHourDepBean>> { data = dlist };
            }
            catch (Exception ex)
            {
                string message = "taskSuccRateDepartment()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<SavingWorkHourDepBean>> { };
            }
        }


        /**
         * 任务数量 top6
         */
        [HttpPost("tasktop6")]
        public BaseResponse<List<TaskTop6Bean>> Tasktop6([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select depname as deptName,TotalTaskCount as taskCount from HUAXIA.t_dashboard_result_Task_Top6Department where update_date=date_format(NOW(),'%Y-%m-%d') ");
                var sql1 = sb1.toString();
                var taskTop6List = conn.Select<TaskTop6Bean>().WithSql(@sql1).ToList();
                return new BaseResponse<List<TaskTop6Bean>> { data = taskTop6List };
            }
            catch (Exception ex)
            {
                string message = "tasktop6()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);

                return new BaseResponse<List<TaskTop6Bean>> { };
            }
        }


        /**
         * 流程数量 top6
         */
        [HttpPost("flowtop6")]
        public BaseResponse<List<FlowTop6Bean>> Flowtop6([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_name as deptName,TotalFlowCount as flowCount from HUAXIA.t_dashboard_result_Flow_Top6Department where update_date=date_format(NOW(),'%Y-%m-%d')" );
                var sql1 = sb1.toString();
                var taskTop6List = conn.Select<FlowTop6Bean>().WithSql(@sql1).ToList();

                return new BaseResponse<List<FlowTop6Bean>> { data = taskTop6List };
            }
            catch (Exception ex)
            {
                string message = "tasktop6()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);

                return new BaseResponse<List<FlowTop6Bean>> { };
            }
        }

        /**
         * 七天任务成功率
         */
        [HttpPost("taskSuccRate7days")]
        public BaseResponse<List<TaskSuccRate7daysBean>> TaskSuccRate7days([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select DATE_FORMAT(query_date,'%Y-%m-%d') as queryDate,taskCount from HUAXIA.t_dashboard_result_TaskSuccRate_7days where update_date=date_format(NOW(),'%Y-%m-%d') ");
                var sql1 = sb1.toString();
                var taskSuccRateList = conn.Select<TaskSuccRate7daysBean>().WithSql(@sql1).ToList();

                return new BaseResponse<List<TaskSuccRate7daysBean>> { data = taskSuccRateList };
            }
            catch (Exception ex)
            {
                string message = "taskSuccRate7days()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskSuccRate7daysBean>> { };
            }
        }

        /**
         * 运行成功率、运行次数
         */
        [HttpPost("taskSuccRate")]
        public BaseResponse<List<TaskSuccRateBean>> TaskSuccRate([FromServices] IFreeSql conn)
        {
            try
            {
                // 七天成功率
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select DATE_FORMAT(query_date,'%Y-%m-%d') as queryDate,taskCount from HUAXIA.t_dashboard_result_TaskSuccRate_7days where update_date=date_format(NOW(),'%Y-%m-%d') ");
                var sql1 = sb1.toString();
                var taskSuccRatekList = conn.Select<TaskSuccRateBean>().WithSql(@sql1).ToList();

                return new BaseResponse<List<TaskSuccRateBean>> { data = taskSuccRatekList };
            }
            catch (Exception ex)
            {
                string message = "TaskSuccRate()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskSuccRateBean>>{ };
            }
        }

        /**
         *  测试达梦数据库连接
         */
        [HttpPost("testDmConnection")]
        public BaseResponse<object> TestDmConnection([FromServices] IFreeSql conn)
        {
            try
            {
                // 测试查询
                StringBuilder sb = new StringBuilder();
                sb.append("SELECT 1 AS test_value FROM DUAL");
                var sql = sb.toString();

                var result = conn.Ado.ExecuteScalar(sql);

                // 获取数据库信息
                var dbInfo = new
                {
                    databaseType = "Dameng/DM8",
                    connectionStatus = "成功",
                    testResult = result,
                    message = "达梦数据库连接测试成功！"
                };

                return new BaseResponse<object>(dbInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "达梦数据库连接测试失败");
                var errorInfo = new
                {
                    databaseType = "Dameng",
                    connectionStatus = "失败",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    message = "达梦数据库连接测试失败，请检查连接字符串和数据库服务状态"
                };
                return new BaseResponse<object>(errorInfo, "500", ex.Message);
            }
        }

        /**
         *  测试达梦数据库表查询
         */
        [HttpPost("testDmTableQuery")]
        public BaseResponse<object> TestDmTableQuery([FromServices] IFreeSql conn)
        {
            try
            {
                // 使用完整表名格式: HUAXIA.user_login
                StringBuilder sb = new StringBuilder();
                sb.append("SELECT COUNT(*) AS user_count FROM HUAXIA.user_login");
                var sql = sb.toString();

                var result = conn.Ado.ExecuteScalar(sql);

                // 获取表信息
                var queryInfo = new
                {
                    tableName = "HUAXIA.user_login",
                    recordCount = result,
                    queryStatus = "成功",
                    message = $"成功查询到HUAXIA.user_login表，共{result}条记录"
                };

                return new BaseResponse<object>(queryInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "达梦数据库表查询测试失败");
                var errorInfo = new
                {
                    tableName = "HUAXIA.user_login",
                    queryStatus = "失败",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    message = "达梦数据库表查询测试失败，请检查表结构"
                };
                return new BaseResponse<object>(errorInfo, "500", ex.Message);
            }
        }

        /**
         *  测试达梦数据库HUAXIA模式下多表查询
         */
        [HttpPost("testDmHuaxiaTables")]
        public BaseResponse<object> TestDmHuaxiaTables([FromServices] IFreeSql conn)
        {
            try
            {
                // 查询HUAXIA模式下的业务表
                var tables = new[]
                {
                    "HUAXIA.user_login",
                    "HUAXIA.t_dashboard_monitor_topinfo",
                    "HUAXIA.t_dashboard_result_topinfo"
                };

                var results = new System.Collections.Generic.List<object>();

                foreach (var table in tables)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.append($"SELECT COUNT(*) FROM {table}");
                        var count = conn.Ado.ExecuteScalar(sb.toString());

                        results.Add(new
                        {
                            tableName = table,
                            recordCount = count,
                            status = "成功"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            tableName = table,
                            recordCount = 0,
                            status = $"失败: {ex.Message}"
                        });
                    }
                }

                var queryInfo = new
                {
                    schema = "HUAXIA",
                    tables = results,
                    message = $"成功查询HUAXIA模式下的{tables.Length}张表"
                };

                return new BaseResponse<object>(queryInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "达梦数据库HUAXIA表查询测试失败");
                var errorInfo = new
                {
                    schema = "HUAXIA",
                    queryStatus = "失败",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    message = "达梦数据库HUAXIA表查询测试失败"
                };
                return new BaseResponse<object>(errorInfo, "500", ex.Message);
            }
        }



    }




}