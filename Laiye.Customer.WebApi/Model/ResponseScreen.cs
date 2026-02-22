
namespace Laiye.Customer.WebApi.Model

{
    // 用户实体
    public class UserLoginBean
    {
        public string userLoginId { get; set; }        // 登录名

        public string currentPassword { get; set; }    // 密码        
    }

    // 日常监控顶部
    public class EverydayTopBean
    {
        public long totalTaskCount { get; set; }        // 本月任务总数（个）

        public long successTaskCount { get; set; }      // 本月任务成功数（个）

        public long totalFlowCount { get; set; }        // 本月新增流程数（个）

        public float saveWorkHour { get; set; }         // 本月节省工时（小时）
    }

    // 今日失败任务数
    public class TotalFailTaskRateBean
    {
        public long failTaskNum { get; set; }      // 失败个数

        public long totalTaskNum { get; set; }     // 总数
   
    }

    // 失败任务
    public class FailTaskRate
    {
        public long todayTaskFailNumber { get; set; }      // 今日失败任务数

        public float taskFailRate { get; set; }            // 失败率        

        public List<FailTaskBean> failTaskList { get; set; }     // 失败任务列表
    }

    // 失败任务
    public class FailTaskBean
    {
        public string queryDate { get; set; }         // 报错时间

        public string deptName { get; set; }          // 部门

        public string workerName { get; set; }        // worker名称

        public string flowName { get; set; }          // 流程名称

        public string content { get; set; }           // 报错信息
    }

    // 机器人分布
    public class RobotDistributionBean
    {  
        public string deptName { get; set; }          // 部门

        public long robotNumber { get; set; }         // 机器人数量       
    }


    // 流程数量
    public class FlowCountBean
    {
        public string queryDate { get; set; }          // 日期

        public long  flowCount { get; set; }           // 流程数量     
    }

    // 任务数量成功率
    public class TaskCountRateBean
    {
        public string queryDate { get; set; }          // 日期

        public long taskCount { get; set; }            // 任务数量

        public float taskRate { get; set; }            // 任务成功率   

    }


    // 任务数量
    public class TaskCountBean
    {
        public string queryDate { get; set; }          // 日期

        public long   taskCount { get; set; }          // 任务数量      

    }

    // 任务成功率
    public class TaskRateBean
    {
        public string queryDate { get; set; }          // 日期

        public float taskRate { get; set; }            // 任务成功率    
    }

    // 新增流程
    public class FlowAddBean
    {
        public string weekName { get; set; }            // 日期

        public long  flowCount { get; set; }            // 流程数量

        public int sortflag { get; set; }               // 排序
    }

    // 近期运行情况
    public class RunningConditionBean
    {
        public List<FlowCountBean> flowCountList { get; set; }         // 流程数量
                                                    
        public List<TaskCountRateBean> taskCountRateList { get; set; }         // 任务数量任务成功率  

        //public List<TaskRateBean> taskRateList { get; set; }           // 任务成功率

        public List<FlowAddBean> flowAddList { get; set; }             // 新增流程      
    }

    // 实时任务监控
    public class RealTimeTaskBean
    {
        public string startTime { get; set; }         // 开始时间

        public string deptName { get; set; }          // 部门

        public string workerName { get; set; }        // worker名称

        public string flowName { get; set; }          // 流程名称

        public string taskId { get; set; }            // 任务ID

        public string taskState { get; set; }         // 状态
    }

    // 机器人在线监控
    public class RobotOnlineParent
    {
        public long   workerOfflineSum { get; set; }     // worker下线人数

        public long   workerCountSum { get; set; }       // worker总数

        public List<RobotOnlineBean> robotOnlineBeansList { get; set; }

    }

    // 机器人在线监控
    public class RobotOnlineBean
    {
        public string deptName { get; set; }          // 部门

        public int workerOffline { get; set; }      // worker在线人数

        public int workerCount { get; set; }       // worker总数

        public float workerOfflineRate { get; set; } // 离线率

        /// <summary>
        /// 调试用，部门下的worker名称
        /// </summary>
        public string workerNames { get; set; }

    }

    // 机器人在线监控
    public class RobotOnline
    {
        public string deptName { get; set; }          // 部门

        public long workerOffline { get; set; }     // worker在线人数

        public long workerCount { get; set; }       // worker总数

        public float workerOfflineRate { get; set; } // 离线率

        public string children { get; set; }         // 子节点
    } 


    // 任务运行统计
    public class TaskRunStatisticsBean
    {  
        public string deptName { get; set; }          // 部门

        public long taskSuccess { get; set; }       // 成功任务

        public long taskFail { get; set; }          // 失败任务

        public long taskRunning { get; set; }       // 运行中任务

        public long taskDeploying { get; set; }      // 待运行任务
    }

    // 任务失败次数Top5统计
    public class TaskFailureTop5
    {
        public string deptName { get; set; }        // 部门

        public string flowName { get; set; }       // 流程名称       

        public long taskFail { get; set; }         // 失败次数

        public string queryTime { get; set; }      // 失败时间
       
    }

    // 任务失败次数Top5统计
    public class TaskFailureTop5Bean
    {
        public string deptName { get; set; }        // 部门

        public string flowName { get; set; }       // 流程名称       

        public long   taskFail { get; set; }       // 失败次数

        public string queryTime { get; set; }      // 失败时间

        public string firstTime { get; set; }       //第一个时间
    }

    // 运行成功率、运行次数
    public class TaskSuccRateBean
    {
        public string queryDate { get; set; }        // 日期

        public long  taskCount { get; set; }         // 运行次数 
       
    }

    // 按部门运行成功率、运行次数
    public class TaskSuccRateDepartmentBean
    {
        public string depId { get; set; }           // ID

        public string deptName { get; set; }         // 部门

        public long totalTask { get; set; }         // 任务总数 

        public long totalSucc { get; set; }         // 任务成功次数

        public float succRatePercent { get; set; }  // 成功率 

    }

    // 节省工时Top10
    public class SavingWorkHourDepBean
    {
        public string depId { get; set; }           // ID

        public string deptName { get; set; }         // 部门      

        public float savingtime { get; set; }        // 节省工时

    }

    // 任务完成排行榜
    public class TaskFinishBean
    {
        public string deptName { get; set; }         // 部门      

        public long workerNum { get; set; }          // worker数量

        public long userNum { get; set; }            // 用户数量

        public long taskNum { get; set; }            // 任务数量

        public long flowNum { get; set; }            // 流程数量

        public float workHour { get; set; }          // 节约成本
    }

    // 部门任务top6
    public class TaskTop6Bean
    {
        public string deptName { get; set; }         // 部门 

        public long  taskCount { get; set; }          // 任务数量        
    }

    // 部门流程top6
    public class FlowTop6Bean
    {
        public string deptName { get; set; }         // 部门 

        public long flowCount { get; set; }          // 流程数量        
    }

    // 七天任务成功率
    public class TaskSuccRate7daysBean
    {
        public string queryDate { get; set; }         // 日期 

        public float taskCount { get; set; }          //  成功率        
    }

    // 流程总数
    public class TaskFlowCountBean
    {
        public long flowCount { get; set; }         // 流程总数            
    }

    // 计划任务数、运行成功次数、失败次数
    public class TaskStatisticsBean
    {
        public long totalTask { get; set; }         // 计划任务数
                                                   
        public long taskSucceed { get; set; }       // 运行成功次数

        public long taskFailed { get; set; }        // 失败次数
    }

    // 节省时间
    public class LaborTimeBean
    {
        public float laborTime { get; set; }         // 节省工时    
    }

    // 今日成功、失败、运行、待运行
    public class TodayRuninfo
    {
        public long todayTasksuccess { get; set; }         // 今日成功数

        public long todayTaskfailed { get; set; }          // 今日失败数

        public long todayTaskrunning { get; set; }         // 今日运行数

        public long todayTaskdeploying { get; set; }       // 今日待运行          

    }

    // 今日成功、失败、运行、待运行、成功率
    public class TodayRuninfoBean
    {
        public long todayTasksuccess { get; set; }         // 今日成功数

        public long todayTaskfailed { get; set; }          // 今日失败数

        public long todayTaskrunning { get; set; }         // 今日运行数

        public long todayTaskdeploying { get; set; }       // 今日待运行  

        public float taskSuccessRate { get; set; }         // 任务成功率         

    }


    // 运行成功次数、流程总数、计划任务数、节省工时
    public class TaskFlowBean
    {
        public long flowCount { get; set; }         // 流程总数

        public long totalTask { get; set; }         // 计划任务数

        public long taskSucceed { get; set; }       // 运行成功次数

        public long taskFailed { get; set; }        // 失败次数

        public long flowDeptcount { get; set; }    // 流程部门数

        public long usercount { get; set; }         // 用户数        

        public float laborTime { get; set; }        // 节省工时 

        public float taskSuccessRate { get; set; }  // 任务成功率         

    }

    // 任务失败原因
    public class TaskFailedReasonBean
    {
        public string errcode { get; set; }          // 失败原因

        public string deptName { get; set; }         // 部门

        public string flowName { get; set; }         // 流程名

        public long errcodeCount { get; set; }       // 失败次数       

    }

    // 任务失败原因
    public class TaskFailedReason
    {
        public long index { get; set; }            // 序号

        public string errcode { get; set; }          // 失败原因

        public string firstErrcode { get; set; }     // 第一条失败原因

        public string deptName { get; set; }         // 部门

        public string flowName { get; set; }         // 流程名

        public long errcodeCount { get; set; }       // 失败次数       

    }


    // 四边形
    public class TaskFlow7dayBean
    {
        public long workerCount { get; set; }          // worker 数量

        public long taskCount { get; set; }            // 任务数量

        public long flowCount { get; set; }            // 流程数量

        public long userCount { get; set; }            // 使用用户数量
                                                      
        public List<TaskSuccRateBean> taskSuccRateList { get; set; }  // 七天成功率

    }

    // 任务运行失败率按机器人
    public class TaskFailedWorkerRateBean
    {
        public string queryDate { get; set; }          // 日期

        public string workerName { get; set; }         // 机器人

        public long taskFailedCount { get; set; }      // 失败总数

        public long taskCount { get; set; }            // 任务总数

        public long workerFailedRate { get; set; }      // 失败率   

    }

    // 任务运行失败率按部门
    public class TaskFailedRateBean
    {
        //public string queryDate { get; set; }          // 日期

        //public string deptName { get; set; }           // 部门

        //public long taskFailedRate { get; set; }       // 失败率   

        public string[] queryDateList { get; set; }       // 日期


        public List<TaskRate> taskRateList { get; set; }         // 部门及每日失败率值    

    }

    public class QueryDateBean {
        public string queryDate { get; set; }          // 日期
    }

    public class DeptNameBean
    {
        public string deptName { get; set; }          // 部门
    }

    public class WorkerNameBean
    {
        public string workerName { get; set; }          // workerName
    }

    public class TaskRate
    {
        public string name { get; set; }          // 部门

        public string type { get; set; } 

        public float[] data { get; set; }
    }

    public class TaskFailedRate
    {
        public string queryDate { get; set; }          // 日期

        public string deptName { get; set; }           // 部门

        public float failedRate { get; set; }          // 失败率
    }

    public class TaskWorkerFailedRate
    {
        public string queryDate { get; set; }          // 日期

        public string workerName { get; set; }         // worker

        public float failedRate { get; set; }          // 失败率
    }

    // 人机交互在线
    public class ManOnlineBean
    {   
        public long manMachineOnline { get; set; }           // 人机交互在线 
    }

    // 人机交互离线
    public class ManNotOnlineBean
    {
        public long manMachineNotOnline { get; set; }        // 人机交互离线
    }

    // 无人值守在线
    public class UnManOnlineBean
    {
        public long unmannedOnline { get; set; }        // 无人值守在线
    }

    // 无人值守离线
    public class UnManNotOnlineBean
    {
        public long unmannedNotOnline { get; set; }        // 无人值守离线
    }

    // worker统计
    public class WorkerBean
    {
        public long manMachineOnline { get; set; }           // 人机交互在线 

        public long manMachineNotOnline { get; set; }        // 人机交互离线

        public long unmannedOnline { get; set; }             // 无人值守离线

        public long unmannedNotOnline { get; set; }          // 无人值守离线
    }

}
