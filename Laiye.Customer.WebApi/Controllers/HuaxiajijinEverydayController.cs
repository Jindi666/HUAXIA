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
using javax.management;
using static Laiye.EntCmd.Service.Core.Worker;
using static Laiye.EntCmd.Service.Core.Flow;
using System.Collections.Immutable;
using System.Threading.Tasks;

/**
 *  日常监控
 */
namespace Laiye.Customer.WebApi.Controllers
{
    [ApiController]
    [Route("huaxia/screen/control")]
    public class HuaxiajijinEverydayController : ControllerBase
    {
        private ILogger<ExtendBussinessController> Logger { get; }
        private IConfiguration Configuration { get; }


        public HuaxiajijinEverydayController(ILogger<ExtendBussinessController> _logger, IConfiguration _configuration)
        {
            Logger = _logger;
            Configuration = _configuration;
        }

        [HttpPost("test")]
        public BaseResponse<string> Test([FromServices] IFreeSql conn)
        {
            return new BaseResponse<string>("Hellow World China!");
        }

        /**
         * 查询表结构 - 临时接口
         */
        [HttpPost("queryTableStructure")]
        public BaseResponse<object> QueryTableStructure([FromServices] IFreeSql conn, [FromBody] Dictionary<string, string> request)
        {
            try
            {
                string tableName = request.ContainsKey("tableName") ? request["tableName"] : "";

                if (string.IsNullOrEmpty(tableName))
                {
                    // 查询所有监控相关的表
                    var tableList = conn.Ado.Query<dynamic>(
                        "SELECT TABLE_NAME as tableName FROM ALL_TABLES WHERE OWNER = 'HUAXIA' AND TABLE_NAME LIKE 't_dashboard%' ORDER BY TABLE_NAME"
                    ).ToList();

                    return new BaseResponse<object> { data = tableList };
                }
                else
                {
                    // 查询指定表的结构
                    var columnList = conn.Ado.Query<dynamic>(
                        $@"SELECT COLUMN_NAME as columnName, DATA_TYPE as dataType,
                          DATA_LENGTH as dataLength, NULLABLE as nullable, COMMENTS as comments
                        FROM ALL_TAB_COLUMNS
                        WHERE TABLE_NAME = '{tableName.ToUpper()}' AND OWNER = 'HUAXIA'
                        ORDER BY COLUMN_ID"
                    ).ToList();

                    return new BaseResponse<object> { data = new { tableName, columns = columnList } };
                }
            }
            catch (Exception ex)
            {
                string message = "queryTableStructure() ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<object> { };
            }
        }

        /**
         * 日常监控顶部
         * 一、本月节省工时（小时）
         * 二、流程总数（个）
         * 三、任务成功数（个）
         * 四、任务总数（个）
         */
        [HttpPost("everydayTop")]
        public BaseResponse<EverydayTopBean> EverydayTop([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select * from  HUAXIA.t_dashboard_monitor_topinfo  where update_date=date_format(NOW(),'%Y-%m-%d') ");

                var sql1 = sb1.toString();
                var query = conn.Select<EverydayTopBean>().WithSql(@sql1).ToOne();

                var everydayTop = new EverydayTopBean
                {
                    totalTaskCount = query.totalTaskCount,
                    successTaskCount = query.successTaskCount,
                    totalFlowCount = query.totalFlowCount,
                    saveWorkHour = float.Parse(query.saveWorkHour.ToString("#0.00")),
                };

                return new BaseResponse<EverydayTopBean> { data = everydayTop };
            }
            catch (Exception ex)
            {
                string message = "everydayTop()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<EverydayTopBean> { };
            }

        }

        /**
         * 今日失败任务数***个，到目前为止任务失败率：5%
         */
        [HttpPost("failureRate")]
        public BaseResponse<FailTaskRate> FailureRate([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select query_date as queryDate,dep_name as deptName,worker_name as workerName,flow_name as flowName,content from HUAXIA.t_dashboard_monitor_today_taskfailed_info  ");
                sb1.append(" where update_date=date_format(NOW(),'%Y-%m-%d') ");
                sb1.append(" and update_time=(select max(update_time) from  ");
                sb1.append(" HUAXIA.t_dashboard_monitor_today_taskfailed_info as td where td.update_date=date_format(NOW(),'%Y-%m-%d')) ");
                var sql1 = sb1.toString();
                var flowCountList = conn.Select<FailTaskBean>().WithSql(@sql1).ToList();

                StringBuilder sb2 = new StringBuilder();
                sb2.append(" select  FailedTaskCountToday as failTaskNum,TotalTaskCountToday as totalTaskNum from  HUAXIA.t_dashboard_task_failed_today where update_date=date_format(NOW(),'%Y-%m-%d') ");

                var sql2 = sb2.toString();
                var taskRate = conn.Select<TotalFailTaskRateBean>().WithSql(@sql2).ToOne();

                // 今日任务失败数
                var failTaskNum = taskRate.failTaskNum;
                var totalTaskNum = taskRate.totalTaskNum;

                // 任务失败率
                float taskFailRate = 0;
                if (totalTaskNum > 0 && failTaskNum != 0) {
                    float taskFailRateTemp = ((float)failTaskNum / (float)totalTaskNum) * 100;
                    taskFailRate = float.Parse(taskFailRateTemp.ToString("#0.00"));
                } else if (failTaskNum == 0 ) {
                    taskFailRate = 100;
                }


                var failTaskRate = new FailTaskRate
                {
                    todayTaskFailNumber = failTaskNum,
                    taskFailRate = taskFailRate,
                    failTaskList = flowCountList,
                };

                return new BaseResponse<FailTaskRate>(failTaskRate);
            } catch (Exception ex)
            {
                string message = "FailureRate()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<FailTaskRate> ();
            }          
        }


        /**
         * 今日失败任务数***个，到目前为止任务失败率：5%,带分页
         */
        [HttpPost("failureRatePage")]
        public BaseResult<PagedData<FailTaskBean>> FailureRatePage([FromServices] IFreeSql conn, [FromBody] PageParamer request)
        {
            try
            {
                int pageSize = request.pageSize;
                int pageIndex = request.pageIndex;
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }

                long totalCount = 0;

                StringBuilder sb1 = new StringBuilder();              
                sb1.append(" select query_date as queryDate,dep_name as deptName,worker_name as workerName,flow_name as flowName,content from HUAXIA.t_dashboard_monitor_today_taskfailed_info  ");
                sb1.append(" where update_date=date_format(NOW(),'%Y-%m-%d') ");
                sb1.append(" and update_time=(select max(update_time) from  ");
                sb1.append(" HUAXIA.t_dashboard_monitor_today_taskfailed_info as td where td.update_date=date_format(NOW(),'%Y-%m-%d')) ");
                var sql1 = sb1.toString();

                var flowCountList = conn.Select<FailTaskBean>().WithSql(@sql1).ToList();
                var query = conn.Select<FailTaskBean>().WithSql(@sql1).Page(pageIndex, pageSize).Count(out totalCount).ToList();

                PagedData<FailTaskBean> paged = new PagedData<FailTaskBean>()
                {

                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = query.ToArray()
                };

                return new BaseResult<PagedData<FailTaskBean>>(paged);
            }
            catch (Exception ex)
            {
                string message = "FailureRatePage()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResult<PagedData<FailTaskBean>>();
            }
        }
          


        /**
         * 近期运行情况（近3个月）
         * 流程数量（按天统计）
         * 任务数量（按天统计）
         * 任务成功率（按天统计）
         * 新增流程（按周统计）
         */
        [HttpPost("operationCondition")]
        public BaseResponse<RunningConditionBean> OsperationCondition([FromServices] IFreeSql conn)
        {
            try
            {            
                // 流程数量
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select DATE_FORMAT(query_date,'%Y-%m-%d') as queryDate,flow_count as flowCount from HUAXIA.t_dashboard_monitor_3month_flowcount  ");
                var sql1 = sb1.toString();
                var flowCountList = conn.Select<FlowCountBean>().WithSql(@sql1).ToList();

                // 任务数量
                StringBuilder sb2 = new StringBuilder();
                sb2.append(" select DATE_FORMAT(query_date,'%Y-%m-%d') as queryDate,task_count as taskCount from HUAXIA.t_dashboard_monitor_3month_taskcount ");
                var sql2 = sb2.toString();
                var taskCountList = conn.Select<TaskCountBean>().WithSql(@sql2).ToList();

                // 任务成功率
                StringBuilder sb3 = new StringBuilder();
                sb3.append(" select DATE_FORMAT(query_date,'%Y-%m-%d') as queryDate,task_rate as taskRate from  HUAXIA.t_dashboard_monitor_3month_taskrate ");
                var sql3 = sb3.toString();
                var taskRateList = conn.Select<TaskRateBean>().WithSql(@sql3).ToList();

                // 新增流程
                StringBuilder sb4 = new StringBuilder();
                sb4.append(" select weekname as weekName,flow_count as flowCount,sortflag from HUAXIA.t_dashboard_monitor_3month_flowaddedinfo order by sortflag DESC ");
                var sql4 = sb4.toString();
                var flowAddList = conn.Select<FlowAddBean>().WithSql(@sql4).ToList();

                List<TaskCountRateBean> taskCountRateList = new List<TaskCountRateBean>();
               
                if (taskCountList != null && taskRateList != null) {
                    for (int i = 0; i < taskCountList.Count; i++)
                    {
                        var task = taskCountList[i];
                        string queryDate = task.queryDate;
                        long taskCount = task.taskCount;
                        var taskCountRate = new TaskCountRateBean();
                        for (int j = 0; j < taskRateList.Count; j++)
                        {
                            var taskRateObj = taskRateList[j];
                            string queryDateRate = taskRateObj.queryDate;
                            float taskRate = taskRateObj.taskRate;
                            if (queryDate.Equals(queryDateRate))
                            {
                                taskCountRate.queryDate = queryDate;
                                taskCountRate.taskCount = taskCount;
                                taskCountRate.taskRate = taskRate;
                                taskCountRateList.Add(taskCountRate);
                            }
                        }
                    }
                }
               

                var runningConditionBean = new RunningConditionBean
                {
                    flowCountList = flowCountList,
                    taskCountRateList = taskCountRateList,
                    //taskRateList = taskRateList,
                    flowAddList = flowAddList,
                };
                return new BaseResponse<RunningConditionBean>(runningConditionBean);
            } catch (Exception ex)
            {
                string message = "operationCondition()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
            }
            return new BaseResponse<RunningConditionBean>();
        }


        /**
         * 机器人分布及在线监控
         */
        [HttpPost("robotOnlineMonitoring")]
        public async Task<BaseResponse<RobotOnlineParent>> RobotOnlineMonitoring([FromServices] IFreeSql conn)
        {
            try
            {
                var results = await conn.Select<RobotOnlineBean>().WithSql(@"SELECT dep_name as deptName,
total as workerCount,
(total - online) as workerOffline,
worker_names as workerNames,
0 as workerOfflineRate
FROM HUAXIA.v_base_worker_count_dept").ToListAsync();

                results.ForEach(item =>
                {
                    item.workerOfflineRate = item.workerCount == 0 ? 0 : (float)item.workerOffline / (float)item.workerCount;
                });

                results = results.OrderByDescending(item => item.workerOfflineRate).ToList();

                var result = new RobotOnlineParent 
                {
                    robotOnlineBeansList = results,
                    workerCountSum = results.Sum(item => item.workerCount),
                    workerOfflineSum = results.Sum(item => item.workerOffline)
                };

                return new BaseResponse<RobotOnlineParent>(result);
            }
            catch (Exception ex)
            {
                string message = "RobotOnlineMonitoring()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<RobotOnlineParent> { };
            }
        }

        /**
         * 实时任务监控
         */
        [HttpPost("realTimeTaskMonitoring")]
        public BaseResponse<List<RealTimeTaskBean>> RealTimeTaskMonitoring([FromServices] IFreeSql conn)
        {
            try
            {               
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select start_time as startTime,dep_name as deptName,worker_name as workerName,flow_name as flowName, ");
                sb1.append(" task_id as taskId,task_state as taskState from  HUAXIA.t_dashboard_monitor_realtime_info ");
                sb1.append(" where update_date=date_format(NOW(),'%Y-%m-%d') and update_time=(select max(update_time) from  ");
                sb1.append(" HUAXIA.t_dashboard_monitor_realtime_info where update_date=date_format(NOW(),'%Y-%m-%d')) limit 60 ");
                var sql1 = sb1.toString();
                var realTimeTaskList = conn.Select<RealTimeTaskBean>().WithSql(@sql1).ToList();

                return new BaseResponse<List<RealTimeTaskBean>> { data = realTimeTaskList };
            }
            catch (Exception ex)
            {
                string message = "RealTimeTaskMonitoring()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<RealTimeTaskBean>> { };
            }
        }

        /**
         * 实时任务监控
         */
        [HttpPost("realTimeTaskMonitoringPage")]
        public BaseResult<PagedData<RealTimeTaskBean>> RealTimeTaskMonitoringPage([FromServices] IFreeSql conn, [FromBody] PageParamer request)
        {
            try
            {
                int pageSize = request.pageSize;
                int pageIndex = request.pageIndex;
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }

                long totalCount = 0;

                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select start_time as startTime,dep_name as deptName,worker_name as workerName,flow_name as flowName, ");
                sb1.append(" task_id as taskId,task_state as taskState from  HUAXIA.t_dashboard_monitor_realtime_info ");
                var sql1 = sb1.toString();

                var query = conn.Select<RealTimeTaskBean>().WithSql(@sql1).Page(pageIndex, pageSize).Count(out totalCount).ToList();

                PagedData<RealTimeTaskBean> paged = new PagedData<RealTimeTaskBean>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = query.ToArray()
                };

                return new BaseResult<PagedData<RealTimeTaskBean>>(paged);                
            }
            catch (Exception ex)
            {
                string message = "realTimeTaskMonitoringPage()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResult<PagedData<RealTimeTaskBean>>();
            }
        }

        /**
         * 任务运行统计
         */
        [HttpPost("taskRunStatistics")]
        public BaseResponse<List<TaskRunStatisticsBean>> TaskRunStatistics([FromServices] IFreeSql conn)
        {
            try
            {
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_name as deptName,task_success as taskSuccess,task_failed as taskFail,task_running as taskRunning, ");
                sb1.append(" task_deploying as taskDeploying from HUAXIA.t_dashboard_monitor_realtime_depttaskinfo   ");
                sb1.append("  where update_date=date_format(NOW(),'%Y-%m-%d') and update_time=(select max(update_time) from   ");
                sb1.append("  HUAXIA.t_dashboard_monitor_realtime_depttaskinfo where update_date=date_format(NOW(),'%Y-%m-%d')) limit 60 ");
                var sql1 = sb1.toString();
                var taskRunStatisticskList = conn.Select<TaskRunStatisticsBean>().WithSql(@sql1).ToList();                

                return new BaseResponse<List<TaskRunStatisticsBean>> { data = taskRunStatisticskList };
            }
            catch (Exception ex)
            {
                string message = "taskRunStatistics()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskRunStatisticsBean>> { };
            }
        }

        /**
         * 任务运行统计
         */
        [HttpPost("taskRunStatisticsPage")]
        public BaseResult<PagedData<TaskRunStatisticsBean>> TaskRunStatisticsPage([FromServices] IFreeSql conn, [FromBody] PageParamer request)
        {
            try
            {
                int pageSize = request.pageSize;
                int pageIndex = request.pageIndex;
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }

                long totalCount = 0;

                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select dep_name as deptName,task_success as taskSuccess,task_failed as taskFail,task_running as taskRunning, ");
                sb1.append(" task_deploying as taskDeploying from HUAXIA.t_dashboard_monitor_realtime_depttaskinfo ");
                sb1.append("  where update_date=date_format(NOW(),'%Y-%m-%d') and update_time=(select max(update_time) from   ");
                sb1.append("  HUAXIA.t_dashboard_monitor_realtime_depttaskinfo where update_date=date_format(NOW(),'%Y-%m-%d')) ");
                var sql1 = sb1.toString();

                var query = conn.Select<TaskRunStatisticsBean>().WithSql(@sql1).Page(pageIndex, pageSize).Count(out totalCount).ToList();

                PagedData<TaskRunStatisticsBean> paged = new PagedData<TaskRunStatisticsBean>()
                {

                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = query.ToArray()
                };

                return new BaseResult<PagedData<TaskRunStatisticsBean>>(paged);
            }
            catch (Exception ex)
            {
                string message = "taskRunStatistics()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResult<PagedData<TaskRunStatisticsBean>>();
            }
        }

        /**
         * 诊断 taskFailureTop5Page 问题
         */
        [HttpPost("diagnoseTaskFailureTop5")]
        public BaseResponse<object> DiagnoseTaskFailureTop5([FromServices] IFreeSql conn)
        {
            try
            {
                var result = new System.Collections.Generic.Dictionary<string, object>();

                // 1. 查询表结构
                StringBuilder sb1 = new StringBuilder();
                sb1.append("SELECT COLUMN_NAME as columnName, DATA_TYPE as dataType, DATA_LENGTH as dataLength FROM ALL_TAB_COLUMNS ");
                sb1.append("WHERE TABLE_NAME = 'T_DASHBOARD_MONITOR_TOP5_TASKFAILEDCOUNT' AND OWNER = 'HUAXIA' ORDER BY COLUMN_ID");
                var columns = conn.Ado.Query<dynamic>(sb1.toString()).ToList();
                result.Add("表结构", columns);

                // 2. 查询所有数据（不做字段映射）
                StringBuilder sb2 = new StringBuilder();
                sb2.append("SELECT * FROM HUAXIA.T_DASHBOARD_MONITOR_TOP5_TASKFAILEDCOUNT ORDER BY UPDATE_DATE DESC");
                var allData = conn.Ado.Query<dynamic>(sb2.toString()).ToList();
                result.Add("所有数据", allData);

                // 3. 查询记录数
                StringBuilder sb3 = new StringBuilder();
                sb3.append("SELECT COUNT(*) as totalCount FROM HUAXIA.T_DASHBOARD_MONITOR_TOP5_TASKFAILEDCOUNT");
                var countResult = conn.Ado.Query<dynamic>(sb3.toString()).ToList();
                result.Add("记录数", countResult);

                // 4. 查询今天的数据
                StringBuilder sb4 = new StringBuilder();
                sb4.append("SELECT * FROM HUAXIA.T_DASHBOARD_MONITOR_TOP5_TASKFAILEDCOUNT WHERE UPDATE_DATE = TO_CHAR(SYSDATE, 'YYYY-MM-DD')");
                var todayData = conn.Ado.Query<dynamic>(sb4.toString()).ToList();
                result.Add("今天的数据", todayData);

                // 5. 测试带别名的查询
                StringBuilder sb5 = new StringBuilder();
                sb5.append("SELECT dep_name as deptName, flow_name as flowName, task_failed_count as taskFail, query_time as queryTime ");
                sb5.append("FROM HUAXIA.T_DASHBOARD_MONITOR_TOP5_TASKFAILEDCOUNT ORDER BY UPDATE_DATE DESC");
                var aliasedData = conn.Ado.Query<dynamic>(sb5.toString()).ToList();
                result.Add("带别名的查询", aliasedData);

                return new BaseResponse<object>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "diagnoseTaskFailureTop5 错误");
                return new BaseResponse<object> {
                    data = new { error = ex.Message, stackTrace = ex.StackTrace }
                };
            }
        }

        /**
         * 任务失败次数Top5统计
         */
        [HttpPost("taskFailureTop5")]
        public BaseResponse<List<TaskFailureTop5Bean>> TaskFailureTop5([FromServices] IFreeSql conn)
        {
            try
            {
                var taskFailureTop5List = new List<TaskFailureTop5Bean>();
                // 任务失败次数Top5
                var sb = new StringBuilder();
                sb.append(@" select dep_name as deptName,flow_name as flowName,task_failed_count as taskFail,query_time as queryTime
from HUAXIA.t_dashboard_monitor_top5_taskfailedcount where update_date=TO_CHAR(SYSDATE, 'YYYY-MM-DD')
");
                var sql = sb.toString();

                var query = conn.Select<TaskFailureTop5>().WithSql(sql).ToList();

                if (query.Any() && query.Min(item => item.taskFail) < 5)
                {
                    // 如果失败次数都小于5，重新查询（逻辑保留但移除不存在的mouth字段）
                    query = conn.Select<TaskFailureTop5>().WithSql(sql).ToList();
                }

                for (int j = 0; j < query.Count; j++)
                {
                    var rawQueryTime = query[j].queryTime ?? "";
                    string[] queryTimeList = rawQueryTime.Split(',');
                    StringBuilder queryTimesb = new StringBuilder();
                    var firstTime = "";

                    // 将时间拆分之后，转换成DateTime，放入List中
                    var dateTimeList = new List<DateTime>();

                    if (queryTimeList.Length > 0 && !string.IsNullOrEmpty(queryTimeList[0]))
                    {
                        for (int k = 0; k < queryTimeList.Length; k++)
                        {
                            var dateTimeString = queryTimeList[k];
                            if (!string.IsNullOrEmpty(dateTimeString) && dateTimeString.Length > 10)
                            {
                                try
                                {
                                    DateTime dt = DateTime.ParseExact(dateTimeString.Trim(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                                    dateTimeList.Add(dt);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogWarning($"时间解析失败: {dateTimeString}, {ex.Message}");
                                }
                            }
                        }

                        // 将List变成降序
                        dateTimeList.Sort((x, y) => -x.CompareTo(y));

                        // 处理时间列表
                        if (dateTimeList.Count > 0)
                        {
                            // 如果只有一个时间，直接使用该时间
                            if (dateTimeList.Count == 1)
                            {
                                DateTime dt = dateTimeList[0];
                                queryTimesb.append(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                                firstTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            // 如果有多个时间，跳过第一个，取后续时间（最多5个）
                            else if (dateTimeList.Count >= 2)
                            {
                                int takeCount = System.Math.Min(5, dateTimeList.Count - 1);
                                List<DateTime> queryTimeListTemp = dateTimeList.GetRange(1, takeCount);
                                queryTimeListTemp.ForEach(delegate (DateTime dateTime)
                                {
                                    queryTimesb.append(dateTime.ToString("yyyy-MM-dd HH:mm:ss")).append(",");
                                });
                                if (queryTimeListTemp.Count > 0)
                                {
                                    firstTime = queryTimeListTemp[0].ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                        }
                    }

                    taskFailureTop5List.Add(new TaskFailureTop5Bean
                    {
                        deptName = query[j].deptName,
                        flowName = query[j].flowName,
                        taskFail = query[j].taskFail,
                        queryTime = queryTimesb.ToString(),
                        firstTime = firstTime,
                    });
                }

                return new BaseResponse<List<TaskFailureTop5Bean>> { data = taskFailureTop5List };
            } catch (Exception ex)
            {
                string message = "taskFailureTop5()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskFailureTop5Bean>> { };
            }
        }


        /**
         * 任务失败次数Top5统计
         */
        [HttpPost("taskFailureTop5Page")]
        public BaseResponse<PagedData<TaskFailureTop5Bean>> TaskFailureTop5Page([FromServices] IFreeSql conn, [FromBody] PageParamer request)
        {
            try
            {
                int pageSize = request.pageSize;
                int pageIndex = request.pageIndex;
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }

                long totalCount = 0;

                var taskFailureTop5List = new List<TaskFailureTop5Bean>();
                // 任务失败次数Top5
                StringBuilder sb = new StringBuilder();
                sb.append(" select dep_name as deptName,flow_name as flowName,task_failed_count as taskFail,query_time as queryTime from HUAXIA.t_dashboard_monitor_top5_taskfailedcount order by update_date desc ");
                var sql = sb.toString();

                Logger.LogWarning($"taskFailureTop5Page SQL: {sql}");

                var query = conn.Select<TaskFailureTop5>().WithSql(sql).Page(pageIndex, pageSize).Count(out totalCount).ToList();

                Logger.LogWarning($"taskFailureTop5Page 查询到 {query.Count} 条原始数据, 总数: {totalCount}");

                for (int j = 0; j < query.Count; j++)
                {
                    var queryTime = query[j].queryTime ?? "";
                    string[] queryTimeList = queryTime.Split(',');
                    StringBuilder queryTimesb = new StringBuilder();
                    var firstTime = "";

                    // 将时间拆分之后，转换成DateTime，放入List中
                    var dateTimeList = new List<DateTime>();

                    if (queryTimeList.Length > 0 && !string.IsNullOrEmpty(queryTimeList[0]))
                    {
                        for (int k = 0; k < queryTimeList.Length; k++)
                        {
                            var dateTimeString = queryTimeList[k];
                            // 添加 null 检查和 Trim
                            if (!string.IsNullOrEmpty(dateTimeString) && dateTimeString.Trim().Length > 10)
                            {
                                try
                                {
                                    DateTime dt = DateTime.ParseExact(dateTimeString.Trim(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                                    dateTimeList.Add(dt);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogWarning($"时间解析失败: {dateTimeString}, {ex.Message}");
                                }
                            }
                        }

                        // 将List变成降序
                        dateTimeList.Sort((x, y) => -x.CompareTo(y));

                        // 处理时间列表
                        if (dateTimeList.Count > 0)
                        {
                            // 如果只有一个时间，直接使用该时间
                            if (dateTimeList.Count == 1)
                            {
                                DateTime dt = dateTimeList[0];
                                queryTimesb.append(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                                firstTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            // 如果有2-5个时间，跳过第一个，取剩余的所有
                            else if (dateTimeList.Count >= 2 && dateTimeList.Count < 6)
                            {
                                List<DateTime> queryTimeListTemp = dateTimeList.GetRange(1, dateTimeList.Count - 1);
                                queryTimeListTemp.ForEach(delegate (DateTime dateTime) {
                                    queryTimesb.append(dateTime.ToString("yyyy-MM-dd HH:mm:ss")).append(",");
                                });
                                if (queryTimeListTemp.Count > 0)
                                {
                                    firstTime = queryTimeListTemp[0].ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                            // 如果有6个或更多时间，跳过第一个，取后续5个
                            else if (dateTimeList.Count >= 6)
                            {
                                List<DateTime> queryTimeListTemp = dateTimeList.GetRange(1, 5);
                                queryTimeListTemp.ForEach(delegate (DateTime dateTime) {
                                    queryTimesb.append(dateTime.ToString("yyyy-MM-dd HH:mm:ss")).append(",");
                                });
                                if (queryTimeListTemp.Count > 0)
                                {
                                    firstTime = queryTimeListTemp[0].ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                        }
                    }

                    taskFailureTop5List.Add(new TaskFailureTop5Bean
                    {
                        deptName = query[j].deptName,
                        flowName = query[j].flowName,
                        taskFail = query[j].taskFail,
                        queryTime = queryTimesb.ToString(),
                        firstTime = firstTime,
                    });
                }

                Logger.LogWarning($"taskFailureTop5Page 最终返回 {taskFailureTop5List.Count} 条数据");

                PagedData<TaskFailureTop5Bean> paged = new PagedData<TaskFailureTop5Bean>()
                {

                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = taskFailureTop5List.ToArray()
                };

                return new BaseResponse<PagedData<TaskFailureTop5Bean>>(paged);
            }
            catch (Exception ex)
            {
                string message = "taskFailureTop5Page()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<PagedData<TaskFailureTop5Bean>> { };
            }
        }

        /**
         * 任务失败原因统计
         */
        [HttpPost("taskFailureReason")]
        public BaseResponse<List<TaskFailedReason>> TaskFailureReason([FromServices] IFreeSql conn)
        {
            try
            {
                var yaskFailedReasonList = new List<TaskFailedReason>();
                StringBuilder sb1 = new StringBuilder();
                sb1.append("select errcode,dept_name as deptName,flow_name as flowName,errcode_count as errcodeCount from HUAXIA.t_dashboard_monitor_failedReason_discount where update_date=TO_CHAR(SYSDATE, 'YYYY-MM-DD')  limit 10 ");
                var sql1 = sb1.toString();
                var query = conn.Select<TaskFailedReasonBean>().WithSql(@sql1).ToList();

                for (int j = 0; j < query.Count; j++)
                {
                    var errcode = query[j].errcode;
                    string[] errcodeList = errcode.Split(',');
                    var firstErrcode = errcodeList[0];

                    yaskFailedReasonList.Add(new TaskFailedReason
                    {
                        index = j + 1,
                        firstErrcode = firstErrcode,
                        errcode = errcode,
                        deptName = query[j].deptName,
                        flowName = query[j].flowName,
                        errcodeCount = query[j].errcodeCount,
                    }); ;
                }

                return new BaseResponse<List<TaskFailedReason>> { data = yaskFailedReasonList };
            }
            catch (Exception ex)
            {
                string message = "taskFailureReason()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskFailedReason>> { };
            }            
        }


        /**
         * 任务失败原因统计
         */
        [HttpPost("taskFailureReasonSum")]
        public BaseResponse<List<TaskFailedReason>> TaskFailureReasonSum([FromServices] IFreeSql conn)
        {
            try
            {
                var yaskFailedReasonList = new List<TaskFailedReason>();
                StringBuilder sb1 = new StringBuilder();
                sb1.append("select errcode,dept_name as deptName,flow_name as flowName,errcode_count as errcodeCount from HUAXIA.t_dashboard_monitor_failedReason_discount where update_date=TO_CHAR(SYSDATE, 'YYYY-MM-DD') ");
                var sql1 = sb1.toString();
                var query = conn.Select<TaskFailedReasonBean>().WithSql(@sql1).ToList();

                for (int j = 0; j < query.Count; j++)
                {
                    var errcode = query[j].errcode;
                    string[] errcodeList = errcode.Split(',');
                    var firstErrcode = errcodeList[0];

                    yaskFailedReasonList.Add(new TaskFailedReason
                    {
                        index = j + 1,
                        firstErrcode = firstErrcode,
                        errcode = errcode,
                        deptName = query[j].deptName,
                        flowName = query[j].flowName,
                        errcodeCount = query[j].errcodeCount,
                    }); ;
                }

                return new BaseResponse<List<TaskFailedReason>> { data = yaskFailedReasonList };
            }
            catch (Exception ex)
            {
                string message = "taskFailureReason()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<List<TaskFailedReason>> { };
            }
        }



        /**
         * 任务失败原因统计
         */
        [HttpPost("taskFailureReasonPage")]
        public BaseResult<PagedData<TaskFailedReason>> TaskFailureReasonPage([FromServices] IFreeSql conn, [FromBody] PageParamer request)
        {
            try
            {
                int pageSize = request.pageSize;
                int pageIndex = request.pageIndex;
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }

                long totalCount = 0;

                var yaskFailedReasonList = new List<TaskFailedReason>();
                StringBuilder sb1 = new StringBuilder();
                sb1.append("select errcode,dept_name as deptName,flow_name as flowName,errcode_count as errcodeCount from HUAXIA.t_dashboard_monitor_failedReason_discount  where update_date=TO_CHAR(SYSDATE, 'YYYY-MM-DD') ");
                var sql1 = sb1.toString();

                var query = conn.Select<TaskFailedReasonBean>().WithSql(@sql1).Page(pageIndex, pageSize).Count(out totalCount).ToList();

                for (int j = 0; j < query.Count; j++)
                {
                    var errcode = query[j].errcode;
                    string[] errcodeList = errcode.Split(',');
                    var firstErrcode = errcodeList[0];

                    yaskFailedReasonList.Add(new TaskFailedReason
                    {
                        index = j + 1,
                        firstErrcode = firstErrcode,
                        errcode = errcode,
                        deptName = query[j].deptName,
                        flowName = query[j].flowName,
                        errcodeCount = query[j].errcodeCount,
                    }); ;
                }

                PagedData<TaskFailedReason> paged = new PagedData<TaskFailedReason>()
                {

                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = yaskFailedReasonList.ToArray()
                };

                return new BaseResult<PagedData<TaskFailedReason>>(paged);
            }
            catch (Exception ex)
            {
                string message = "taskFailureReasonPage()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResult<PagedData<TaskFailedReason>> { };
            }
        }

        /**
         * 任务运行失败率按部门
         */
        [HttpPost("taskFailureDeptRate")]
        public BaseResponse<TaskFailedRateBean> TaskFailureDeptRate([FromServices] IFreeSql conn)
        {
            try
            {
                // 获取日期
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select  DISTINCT date_format(query_date,'%Y-%m-%d')  as queryDate  from HUAXIA.t_dashboard_monitor_7days_dept_failedrate order by query_date ASC ");                
                var sql1 = sb1.toString();
                var queryDateBeanList = conn.Select<QueryDateBean>().WithSql(@sql1).ToList();

                string[] queryDateArray = new string[queryDateBeanList.Count];
                for (int j = 0; j < queryDateBeanList.Count; j++)
                {
                    queryDateArray[j] = queryDateBeanList[j].queryDate;
                }

                var taskRateList = new List<TaskRate>();
                // 获取部门
                StringBuilder sb2 = new StringBuilder();
                sb2.append("  select  DISTINCT dep_name  as deptName from HUAXIA.t_dashboard_monitor_7days_dept_failedrate ");
                var sql2 = sb2.toString();
                var deptNameList = conn.Select<DeptNameBean>().WithSql(sql2).ToList();


                // 获取失败率 
                StringBuilder sb3 = new StringBuilder();
                sb3.append(" select  query_date as queryDate,dep_name as deptName,task_failed_rate as failedRate  from HUAXIA.t_dashboard_monitor_7days_dept_failedrate  order by query_date ASC ");
                var sql3 = sb3.toString();
                var taskFailedRateList = conn.Select<TaskFailedRate>().WithSql(sql3).ToList();


                for (int j = 0; j < deptNameList.Count; j++)
                {
                    string deptNameValue = deptNameList[j].deptName;  
                    
                    var rateList = new List<float>();
                    for (int k = 0;k < taskFailedRateList.Count;k++) {
                        string deptName = taskFailedRateList[k].deptName;
                        if (deptNameValue.Equals(deptName)) {                           
                            rateList.Add(taskFailedRateList[k].failedRate);
                        }
                    }
                   
                    float rate1 = rateList[0];
                    float rate2 = rateList[1]; 
                    float rate3 = rateList[2];
                    float rate4 = rateList[3];                        
                    float rate5 = rateList[4];                        
                    float rate6 = rateList[5];
                    float rate7 = rateList[6];
                    float rate8 = rateList[7];
                    float rate9 = rateList[8];
                    float rate10 = rateList[9];
                    float rate11 = rateList[10];
                    float rate12 = rateList[11];
                    float rate13 = rateList[12];
                    float rate14 = rateList[13];

                    if (rate1 == 0 && rate2 == 0 && rate3 == 0 && rate4 == 0 && rate5 == 0 && rate6 == 0 &&
                        rate7 == 0 && rate8 == 0 && rate9 == 0 && rate10 == 0 && rate11 == 0 && rate12 == 0 && rate13 == 0 && rate14 == 0)
                    {
                        Console.WriteLine(deptNameValue);
                    } else {
                        taskRateList.Add(new TaskRate
                        {
                            name = deptNameValue,
                            type = "line",
                            data = rateList.ToArray(),
                        });
                    }
                }

                var taskFailedRateBean = new TaskFailedRateBean
                {
                    queryDateList = queryDateArray,
                    taskRateList = taskRateList,
                };

                return new BaseResponse<TaskFailedRateBean> (taskFailedRateBean);
            }
            catch (Exception ex)
            {
                string message = "taskFailureRate()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<TaskFailedRateBean> ();
            }
        }

        /**
         * 任务运行失败率按机器人
         */
        [HttpPost("taskFailureWorkerRate")]
        public BaseResponse<TaskFailedRateBean> TaskFailureWorkerRate([FromServices] IFreeSql conn)
        {
            try
            {
                // 获取日期
                StringBuilder sb1 = new StringBuilder();
                sb1.append(" select  DISTINCT date_format(query_date,'%Y-%m-%d')  as queryDate  from HUAXIA.t_dashboard_monitor_7days_worker_failedrate order by query_date ASC ");
                var sql1 = sb1.toString();
                var queryDateBeanList = conn.Select<QueryDateBean>().WithSql(@sql1).ToList();

                string[] queryDateArray = new string[queryDateBeanList.Count];
                for (int j = 0; j < queryDateBeanList.Count; j++)
                {
                    queryDateArray[j] = queryDateBeanList[j].queryDate;
                }

                var taskRateList = new List<TaskRate>();
                // 获取worker
                StringBuilder sb2 = new StringBuilder();
                sb2.append("  select  DISTINCT worker_name  as workerName from HUAXIA.t_dashboard_monitor_7days_worker_failedrate   ");
                var sql2 = sb2.toString();
                var workerNameList = conn.Select<WorkerNameBean>().WithSql(sql2).ToList();

                // 获取失败率 
                StringBuilder sb3 = new StringBuilder();
                sb3.append(" select  query_date as queryDate,worker_name as workerName,worker_failedrate as failedRate  from HUAXIA.t_dashboard_monitor_7days_worker_failedrate  order by query_date ASC ");
                var sql3 = sb3.toString();
                var taskFailedRateList = conn.Select<TaskWorkerFailedRate>().WithSql(sql3).ToList();


                for (int j = 0; j < workerNameList.Count; j++)
                {
                    string workerNameValue = workerNameList[j].workerName;

                    var rateList = new List<float>();
                    for (int k = 0; k < taskFailedRateList.Count; k++)
                    {
                        string deptName = taskFailedRateList[k].workerName;
                        if (workerNameValue.Equals(deptName))
                        {
                            rateList.Add(taskFailedRateList[k].failedRate);
                        }
                    }
                    taskRateList.Add(new TaskRate
                    {
                        name = workerNameValue,
                        type = "line",                     
                        data = rateList.ToArray(),
                    });
                }

                var taskFailedRateBean = new TaskFailedRateBean
                {
                    queryDateList = queryDateArray,
                    taskRateList = taskRateList,
                };

                return new BaseResponse<TaskFailedRateBean>(taskFailedRateBean);
            }
            catch (Exception ex)
            {
                string message = "taskFailureWorkerRate()" + "ex:" + ex.Message;
                Logger.LogWarning(ex, ex.Message);
                return new BaseResponse<TaskFailedRateBean> { };
            }
        }

    }
}
