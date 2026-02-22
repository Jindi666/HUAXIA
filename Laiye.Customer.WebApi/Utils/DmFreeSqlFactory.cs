using FreeSql;
using System.Text.RegularExpressions;

namespace Laiye.Customer.WebApi.Utils
{
    /// <summary>
    /// 达梦数据库FreeSql工厂类
    /// 直接使用FreeSql.Provider.Dameng,自动转换MySQL语法到达梦数据库语法
    /// 自动为表名添加对应的Schema前缀(HUAXIA或RPA)
    /// </summary>
    public static class DmFreeSqlFactory
    {
        /// <summary>
        /// HUAXIA模式下的表列表
        /// </summary>
        private static readonly string[] HuaxiaTables = new[]
        {
            "user_login",
            "t_dashboard_monitor_topinfo",
            "t_dashboard_result_topinfo",
            "t_dashboard_result_topinfo_today_runinfo",
            "t_dashboard_monitor_realtime_info",
            "t_dashboard_monitor_realtime_depttaskinfo",
            "t_dashboard_monitor_3month_flowcount",
            "t_dashboard_monitor_3month_flowaddedinfo",
            "t_dashboard_monitor_3month_taskcount",
            "t_dashboard_monitor_3month_taskrate",
            "t_dashboard_monitor_today_taskfailed_info",
            "t_dashboard_monitor_7days_dept_failedrate",
            "t_dashboard_monitor_7days_worker_failedrate",
            "t_dashboard_monitor_failedreason_discount",
            "t_dashboard_monitor_top5_taskfailedcount",
            "t_dashboard_monitor_worker_dept_offline",
            "t_dashboard_result_7day_info",
            "t_dashboard_result_taskfinish_department",
            "t_dashboard_result_tasksuccrate_department",
            "t_dashboard_result_tasksuccrate_7days",
            "t_dashboard_result_flow_top6department",
            "t_dashboard_result_task_top6department",
            "t_dashboard_result_savingworkhourdep_top10",
            "t_dashboard_task_failed_today",
            "t_base_worker",
            "t_base_deparment",
            "t_base_dept_taskfailed_count",
            "t_base_worker_taskfailed_count",
            "v_base_worker_count_dept",
            "tbl_dashboard_task_execute_log",
            "test_event"
        };

        /// <summary>
        /// RPA模式下的表列表(原MySQL中的uibot_rpa库)
        /// 注意：需要同时转换schema和表名
        /// - MySQL: uibot_rpa.tbl_user_worker -> 达梦: RPA.TBL_CMD_ATTENDED_WORKER
        /// - MySQL: uibot_rpa.tbl_worker -> 达梦: RPA.TBL_CMD_WORKER
        /// </summary>
        private static readonly string[] RpaTables = new[]
        {
            "tbl_user_worker",   // 需要映射到 RPA.TBL_CMD_ATTENDED_WORKER
            "tbl_worker"         // 需要映射到 RPA.TBL_CMD_WORKER
        };

        /// <summary>
        /// RPA表名映射字典(MySQL表名 -> 达梦表名)
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<string, string> RpaTableMapping = new()
        {
            { "tbl_user_worker", "TBL_CMD_ATTENDED_WORKER" },
            { "tbl_worker", "TBL_CMD_WORKER" }
        };

        /// <summary>
        /// 创建支持达梦数据库的IFreeSql实例
        /// </summary>
        /// <param name="connectionString">达梦数据库连接字符串</param>
        /// <returns>IFreeSql实例</returns>
        public static IFreeSql CreateDamengFreeSql(string connectionString)
        {
            Console.WriteLine("检测到达梦数据库，使用FreeSql.Provider.Dameng...");
            Console.WriteLine($"连接字符串: {connectionString}");
            Console.WriteLine("启用MySQL到达梦数据库SQL语法自动转换");
            Console.WriteLine($"Schema映射: HUAXIA({HuaxiaTables.Length}张表), RPA({RpaTables.Length}张表)");

            var fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.Dameng, connectionString)
                .UseAutoSyncStructure(false)
                .UseMonitorCommand(cmd => {
                    // 自动转换SQL语法
                    var originalSql = cmd.CommandText;
                    var modifiedSql = ConvertMySqlToDameng(originalSql);
                    cmd.CommandText = modifiedSql;
                    Console.WriteLine($"SQL: {cmd.CommandText}");
                })
                .UseNoneCommandParameter(true)
                .Build();

            Console.WriteLine($"FreeSql 已创建（版本 3.2.833），数据库类型：Dameng");

            return fsql;
        }

        /// <summary>
        /// 将MySQL语法转换为达梦数据库语法
        /// </summary>
        /// <param name="mySql">MySQL SQL语句</param>
        /// <returns>达梦数据库 SQL语句</returns>
        private static string ConvertMySqlToDameng(string mySql)
        {
            if (string.IsNullOrWhiteSpace(mySql))
                return mySql;

            var result = mySql;

            // 1. 去除MySQL的反引号
            result = result.Replace("`", "");

            // 2. 转换DATE_FORMAT函数
            // DATE_FORMAT(date, '%Y-%m-%d') -> TO_CHAR(date, 'YYYY-MM-DD')
            result = Regex.Replace(result,
                @"DATE_FORMAT\(([^,]+),\s*'%Y-%m-%d'\)",
                "TO_CHAR($1, 'YYYY-MM-DD')",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            result = Regex.Replace(result,
                @"DATE_FORMAT\(([^,]+),\s*'%Y-%m-%d %H:%i:%s'\)",
                "TO_CHAR($1, 'YYYY-MM-DD HH24:MI:SS')",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            result = Regex.Replace(result,
                @"DATE_FORMAT\(([^,]+),\s*'%Y-%m-%d %H:%i:%s\.%f'\)",
                "TO_CHAR($1, 'YYYY-MM-DD HH24:MI:SS.FF3')",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 3. 转换NOW()函数
            result = Regex.Replace(result, @"\bNOW\(\)", "SYSDATE", RegexOptions.IgnoreCase);

            // 4. 转换LIMIT子句
            // 达梦数据库不支持LIMIT,使用ROWNUM或FETCH FIRST
            // 简单处理: 在子查询中,LIMIT n ) a格式转换为 ) a WHERE ROWNUM <= n
            // 更复杂的场景需要在外层查询中添加ROWNUM条件

            // 处理子查询中的LIMIT: LIMIT n ) a -> ) a WHERE ROWNUM <= n
            result = Regex.Replace(result,
                @"LIMIT\s+(\d+)\s*\)\s*a",
                ") a WHERE ROWNUM <= $1",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 处理简单的LIMIT在子查询末尾的情况
            result = Regex.Replace(result,
                @"LIMIT\s+(\d+)\s*$",
                "ROWNUM <= $1",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 5. 添加Schema前缀
            result = AddSchemaPrefix(result);

            // 6. 移除FreeSql自动生成的字段别名双引号
            // FreeSql生成的格式: A."fieldName" -> 达梦需要: A.fieldName 或 A."FIELDNAME"
            // 简化处理: 移除字段别名的双引号
            result = Regex.Replace(result, @"""(\w+)""", "$1", RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>
        /// 为表名添加对应的Schema前缀
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>添加了Schema前缀的SQL语句</returns>
        private static string AddSchemaPrefix(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            var result = sql;

            // 先统一已存在的schema为大写(达梦数据库对大小写敏感)
            result = Regex.Replace(result, @"uibot_rpa\.", "RPA.", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"rpa\.", "RPA.", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"huaxia\.", "HUAXIA.", RegexOptions.IgnoreCase);

            // 处理RPA表的schema和表名映射
            foreach (var mapping in RpaTableMapping)
            {
                var oldTable = mapping.Key;
                var newTable = mapping.Value;

                // 匹配 uibot_rpa.tbl_user_worker 或 rpa.tbl_user_worker
                // 替换为 RPA.TBL_CMD_ATTENDED_WORKER
                var pattern = $@"(?<=\bRPA\.){Regex.Escape(oldTable)}(?=[\s),]|$)";
                result = Regex.Replace(result, pattern, newTable, RegexOptions.IgnoreCase);
            }

            // 已经有正确schema前缀的表名不需要处理
            if (Regex.IsMatch(result, @"(HUAXIA|RPA)\.", RegexOptions.IgnoreCase))
            {
                // 检查是否有表没有schema前缀,如果有则继续处理
                foreach (var table in HuaxiaTables)
                {
                    // 匹配没有schema前缀的表名
                    var pattern = $@"(?<=[\s,(])(?!(HUAXIA|RPA)\.){Regex.Escape(table)}(?=[\s),]|$)";
                    result = Regex.Replace(result, pattern, $"HUAXIA.{table}", RegexOptions.IgnoreCase);
                }

                foreach (var table in RpaTables)
                {
                    var pattern = $@"(?<=[\s,(])(?!(HUAXIA|RPA)\.){Regex.Escape(table)}(?=[\s),]|$)";
                    result = Regex.Replace(result, pattern, $"RPA.{table}", RegexOptions.IgnoreCase);
                }
            }
            else
            {
                // 没有任何schema前缀,为所有表添加前缀
                foreach (var table in HuaxiaTables)
                {
                    var pattern = $@"(?<=[\s,(]){Regex.Escape(table)}(?=[\s),]|$)";
                    result = Regex.Replace(result, pattern, $"HUAXIA.{table}", RegexOptions.IgnoreCase);
                }

                foreach (var table in RpaTables)
                {
                    var pattern = $@"(?<=[\s,(]){Regex.Escape(table)}(?=[\s),]|$)";
                    result = Regex.Replace(result, pattern, $"RPA.{table}", RegexOptions.IgnoreCase);
                }
            }

            return result;
        }
    }
}

