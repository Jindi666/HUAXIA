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
                string sql1 = " SELECT TO_CHAR(query_date, 'HH24:MI:SS') AS queryDate, " +
                              " dep_name AS deptName, worker_name AS workerName, " +
                              " flow_name AS flowName, content " +
                              " FROM HUAXIA.t_dashboard_monitor_today_taskfailed_info " +
                              " WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD') " +
                              " AND update_time = ( " +
                              "     SELECT MAX(update_time) " +
                              "     FROM HUAXIA.t_dashboard_monitor_today_taskfailed_info AS td " +
                              "     WHERE td.update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD')" +
                              " )";

                var flowCountList = conn.Select<FailTaskBean>().WithSql(sql1).ToList();

                string sql2 = " SELECT FailedTaskCountToday AS failTaskNum, TotalTaskCountToday AS totalTaskNum " +
                              " FROM HUAXIA.t_dashboard_task_failed_today " +
                              " WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD')";

                var taskRate = conn.Select<TotalFailTaskRateBean>().WithSql(sql2).ToOne();

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

                string sql1 = " SELECT TO_CHAR(query_date, 'HH24:MI:SS') AS queryDate, " +
                              " dep_name AS deptName, worker_name AS workerName, " +
                              " flow_name AS flowName, content " +
                              " FROM HUAXIA.t_dashboard_monitor_today_taskfailed_info " +
                              " WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD') " +
                              " AND update_time = ( " +
                              "     SELECT MAX(update_time) " +
                              "     FROM HUAXIA.t_dashboard_monitor_today_taskfailed_info AS td " +
                              "     WHERE td.update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD')" +
                              " )";

                var flowCountList = conn.Select<FailTaskBean>().WithSql(sql1).ToList();
                var query = conn.Select<FailTaskBean>().WithSql(sql1).Page(pageIndex, pageSize).Count(out totalCount).ToList();

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
         * 使用视图：V_BASE_WORKER_DEPT_OFFLINE
         * 字段映射：
         *   - DEP_NAME → deptName
         *   - WORKER_COUNT → workerCount
         *   - WORKER_OFFLINE → workerOffline
         *   - WORKER_OFFLINE_RATE → workerOfflineRate
         *   - workerNames（缺失字段，使用空字符串作为固定值）
         */
        [HttpPost("robotOnlineMonitoring")]
        public async Task<BaseResponse<RobotOnlineParent>> RobotOnlineMonitoring([FromServices] IFreeSql conn)
        {
            try
            {
                var results = await conn.Select<RobotOnlineBean>().WithSql(@"
                    SELECT
                        DEP_NAME AS deptName,
                        WORKER_COUNT AS workerCount,
                        WORKER_OFFLINE AS workerOffline,
                        WORKER_OFFLINE_RATE AS workerOfflineRate,
                        NULL AS workerNames
                    FROM HUAXIA.V_BASE_WORKER_DEPT_OFFLINE
                ").ToListAsync();

                // 清空 workerNames（前端不需要此字段）
                results.ForEach(r => r.workerNames = null);

                // 按 offlineRate 降序排序
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
                Logger.LogWarning(ex, "RobotOnlineMonitoring() error: {Message}", ex.Message);
                return new BaseResponse<RobotOnlineParent> { };
            }
        }

        /**
         * 实时任务监控
         * 查询逻辑：获取今天最新一次更新时间的实时任务监控数据，最多返回60条
         * 1. 筛选 update_date = 今天
         * 2. 筛选 update_time = 今天最大的更新时间（即最后一次更新的数据快照）
         */
        [HttpPost("realTimeTaskMonitoring")]
        public BaseResponse<List<RealTimeTaskBean>> RealTimeTaskMonitoring([FromServices] IFreeSql conn)
        {
            try
            {
                string sql = "SELECT TO_CHAR(start_time, 'HH24:MI:SS') AS startTime, " +
                             "dep_name AS deptName, worker_name AS workerName, " +
                             "flow_name AS flowName, task_id AS taskId, task_state AS taskState " +
                             "FROM HUAXIA.t_dashboard_monitor_realtime_info " +
                             "WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD') " +
                             "AND update_time = ( " +
                             "    SELECT MAX(update_time) " +
                             "    FROM HUAXIA.t_dashboard_monitor_realtime_info " +
                             "    WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD')" +
                             ") " +
                             "FETCH FIRST 60 ROWS ONLY";

                var realTimeTaskList = conn.Select<RealTimeTaskBean>().WithSql(sql).ToList();

                return new BaseResponse<List<RealTimeTaskBean>> { data = realTimeTaskList };
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "RealTimeTaskMonitoring() error: {Message}", ex.Message);
                return new BaseResponse<List<RealTimeTaskBean>> { };
            }
        }

        /**
         * 实时任务监控（分页）
         * 查询逻辑：与 realTimeTaskMonitoring 保持一致，只查询今天最新快照的数据进行分页
         * 1. 筛选 update_date = 今天
         * 2. 筛选 update_time = 今天最大的更新时间（即最后一次更新的数据快照）
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

                string sql = "SELECT TO_CHAR(start_time, 'HH24:MI:SS') AS startTime, " +
                             "dep_name AS deptName, worker_name AS workerName, " +
                             "flow_name AS flowName, task_id AS taskId, task_state AS taskState " +
                             "FROM HUAXIA.t_dashboard_monitor_realtime_info " +
                             "WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD') " +
                             "AND update_time = ( " +
                             "    SELECT MAX(update_time) " +
                             "    FROM HUAXIA.t_dashboard_monitor_realtime_info " +
                             "    WHERE update_date = TO_CHAR(SYSDATE, 'YYYY-MM-DD')" +
                             ")";

                var query = conn.Select<RealTimeTaskBean>().WithSql(sql).Page(pageIndex, pageSize).Count(out totalCount).ToList();

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
                Logger.LogWarning(ex, "RealTimeTaskMonitoringPage() error: {Message}", ex.Message);
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
                string sql1 = "SELECT DISTINCT TO_CHAR(query_date, 'YYYY-MM-DD') AS queryDate FROM HUAXIA.t_dashboard_monitor_7days_dept_failedrate ORDER BY queryDate ASC";
                var queryDateBeanList = conn.Ado.Query<QueryDateBean>(sql1).ToList();

                string[] queryDateArray = new string[queryDateBeanList.Count];
                for (int j = 0; j < queryDateBeanList.Count; j++)
                {
                    queryDateArray[j] = queryDateBeanList[j].queryDate;
                }

                var taskRateList = new List<TaskRate>();
                // 获取部门
                string sql2 = "SELECT DISTINCT dep_name AS deptName FROM HUAXIA.t_dashboard_monitor_7days_dept_failedrate";
                var deptNameList = conn.Ado.Query<DeptNameBean>(sql2).ToList();

                // 获取失败率
                string sql3 = "SELECT query_date AS queryDate, dep_name AS deptName, task_failed_rate AS failedRate FROM HUAXIA.t_dashboard_monitor_7days_dept_failedrate ORDER BY queryDate ASC";
                var taskFailedRateList = conn.Ado.Query<TaskFailedRate>(sql3).ToList();

                for (int j = 0; j < deptNameList.Count; j++)
                {
                    string deptNameValue = deptNameList[j].deptName;

                    var rateList = new List<float>();
                    for (int k = 0; k < taskFailedRateList.Count; k++)
                    {
                        string deptName = taskFailedRateList[k].deptName;
                        if (deptNameValue.Equals(deptName))
                        {
                            rateList.Add(taskFailedRateList[k].failedRate);
                        }
                    }

                    taskRateList.Add(new TaskRate
                    {
                        name = deptNameValue,
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
                Logger.LogError(ex, "taskFailureDeptRate 执行失败: {Message}", ex.Message);
                return new BaseResponse<TaskFailedRateBean> { };
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
                string sql1 = "SELECT DISTINCT TO_CHAR(QUERY_DATE, 'YYYY-MM-DD') AS queryDate FROM HUAXIA.T_DASHBOARD_MONITOR_7DAYS_WORKER_FAILEDRATE ORDER BY queryDate ASC";
                var queryDateBeanList = conn.Ado.Query<QueryDateBean>(sql1).ToList();

                string[] queryDateArray = new string[queryDateBeanList.Count];
                for (int j = 0; j < queryDateBeanList.Count; j++)
                {
                    queryDateArray[j] = queryDateBeanList[j].queryDate;
                }

                var taskRateList = new List<TaskRate>();
                // 获取worker
                string sql2 = "SELECT DISTINCT WORKER_NAME AS workerName FROM HUAXIA.T_DASHBOARD_MONITOR_7DAYS_WORKER_FAILEDRATE";
                var workerNameList = conn.Ado.Query<WorkerNameBean>(sql2).ToList();

                // 获取失败率
                string sql3 = "SELECT QUERY_DATE AS queryDate, WORKER_NAME AS workerName, WORKER_FAILEDRATE AS failedRate FROM HUAXIA.T_DASHBOARD_MONITOR_7DAYS_WORKER_FAILEDRATE ORDER BY queryDate ASC";
                var taskFailedRateList = conn.Ado.Query<TaskWorkerFailedRate>(sql3).ToList();

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
                Logger.LogError(ex, "taskFailureWorkerRate 执行失败: {Message}", ex.Message);
                return new BaseResponse<TaskFailedRateBean> { };
            }
        }

    }
}
