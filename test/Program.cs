using System;
using Dm;

namespace DmConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connectionString = "Server=localhost:Port=5236;UserId=SYSDBA;PWD=Zj20031234000;";
                var todayDate = DateTime.Now.ToString("yyyy-MM-dd");

                Console.WriteLine($"查询HUAXIA数据库中今日({todayDate})的测试数据...\n");

                using (var connection = new DmConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("✓ 连接成功!\n");

                    // 查询 t_dashboard_monitor_topinfo
                    Console.WriteLine("=== 1. 查询 t_dashboard_monitor_topinfo ===");
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            SELECT UPDATE_DATE, TOTALFLOWCOUNT, SAVEWORKHOUR, TOTALTASKCOUNT, SUCCESSTASKCOUNT
                            FROM HUAXIA.T_DASHBOARD_MONITOR_TOPINFO
                            WHERE UPDATE_DATE = '{todayDate}'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"  日期: {reader[0]}, 总流程数: {reader[1]}, 节省工时: {reader[2]}, 总任务数: {reader[3]}, 成功任务数: {reader[4]}");
                            }
                        }
                    }

                    // 查询 t_dashboard_result_topinfo_today_runinfo
                    Console.WriteLine("\n=== 2. 查询 t_dashboard_result_topinfo_today_runinfo ===");
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            SELECT UPDATE_DATE, UPDATE_TIME, TODAY_TASKSUCCESS, TODAY_TASKFAILED, TODAY_TASKRUNNING, TODAY_TASKDEPLOYING
                            FROM HUAXIA.T_DASHBOARD_RESULT_TOPINFO_TODAY_RUNINFO
                            WHERE UPDATE_DATE = '{todayDate}'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"  日期: {reader[0]}, 时间: {reader[1]}, 成功: {reader[2]}, 失败: {reader[3]}, 运行中: {reader[4]}, 部署中: {reader[5]}");
                            }
                        }
                    }

                    // 查询 t_dashboard_monitor_realtime_info
                    Console.WriteLine("\n=== 3. 查询 t_dashboard_monitor_realtime_info ===");
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            SELECT UPDATE_DATE, UPDATE_TIME, DEP_NAME, WORKER_NAME, FLOW_NAME, TASK_ID, TASK_STATE
                            FROM HUAXIA.T_DASHBOARD_MONITOR_REALTIME_INFO
                            WHERE UPDATE_DATE = '{todayDate}'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"  {reader[2]} - {reader[3]} - {reader[4]} (任务ID: {reader[5]}, 状态: {reader[6]})");
                            }
                        }
                    }

                    // 查询 t_dashboard_monitor_realtime_depttaskinfo
                    Console.WriteLine("\n=== 4. 查询 t_dashboard_monitor_realtime_depttaskinfo ===");
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            SELECT UPDATE_DATE, UPDATE_TIME, DEP_NAME, TASK_SUCCESS, TASK_FAILED, TASK_RUNNING, TASK_DEPLOYING
                            FROM HUAXIA.T_DASHBOARD_MONITOR_REALTIME_DEPTTASKINFO
                            WHERE UPDATE_DATE = '{todayDate}'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"  {reader[2]} - 成功: {reader[3]}, 失败: {reader[4]}, 运行中: {reader[5]}, 部署中: {reader[6]}");
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n错误: {ex.Message}");
                Console.WriteLine($"堆栈: {ex.StackTrace}");
            }
        }
    }
}
