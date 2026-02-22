-- =====================================================
-- 查询华夏基金大屏相关表的结构
-- =====================================================

-- 设置当前 Schema
SET SCHEMA HUAXIA;

-- =====================================================
-- 查询方法1: 使用达梦数据库的系统视图查询表结构
-- =====================================================

-- 查询 t_dashboard_monitor_topinfo 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_topinfo'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_task_failed_today 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_task_failed_today'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_today_taskfailed_info 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_today_taskfailed_info'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_3month_flowcount 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_3month_flowcount'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_3month_taskcount 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_3month_taskcount'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_3month_taskrate 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_3month_taskrate'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_3month_flowaddedinfo 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_3month_flowaddedinfo'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_realtime_info 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_realtime_info'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_realtime_depttaskinfo 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_realtime_depttaskinfo'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_top5_taskfailedcount 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_top5_taskfailedcount'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_failedReason_discount 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_failedReason_discount'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_7days_dept_failedrate 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_7days_dept_failedrate'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;

-- 查询 t_dashboard_monitor_7days_worker_failedrate 表结构
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    NULLABLE,
    COMMENTS
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME = 't_dashboard_monitor_7days_worker_failedrate'
    AND OWNER = 'HUAXIA'
ORDER BY COLUMN_ID;


-- =====================================================
-- 查询方法2: 使用 DESC 命令 (在达梦数据库客户端中使用)
-- =====================================================
-- 复制以下命令在达梦数据库管理工具中执行:

/*
DESC HUAXIA.t_dashboard_monitor_topinfo;
DESC HUAXIA.t_dashboard_task_failed_today;
DESC HUAXIA.t_dashboard_monitor_today_taskfailed_info;
DESC HUAXIA.t_dashboard_monitor_3month_flowcount;
DESC HUAXIA.t_dashboard_monitor_3month_taskcount;
DESC HUAXIA.t_dashboard_monitor_3month_taskrate;
DESC HUAXIA.t_dashboard_monitor_3month_flowaddedinfo;
DESC HUAXIA.t_dashboard_monitor_realtime_info;
DESC HUAXIA.t_dashboard_monitor_realtime_depttaskinfo;
DESC HUAXIA.t_dashboard_monitor_top5_taskfailedcount;
DESC HUAXIA.t_dashboard_monitor_failedReason_discount;
DESC HUAXIA.t_dashboard_monitor_7days_dept_failedrate;
DESC HUAXIA.t_dashboard_monitor_7days_worker_failedrate;
*/


-- =====================================================
-- 查询方法3: 列出 HUAXIA Schema 下所有表
-- =====================================================
SELECT
    TABLE_NAME,
    COMMENTS
FROM ALL_TABLES
WHERE OWNER = 'HUAXIA'
    AND TABLE_NAME LIKE 't_dashboard%'
ORDER BY TABLE_NAME;


-- =====================================================
-- 查询方法4: 使用 sp_tabledef 存储过程
-- =====================================================
-- 获取表的完整定义
/*
CALL sp_tabledef('HUAXIA', 't_dashboard_monitor_topinfo');
CALL sp_tabledef('HUAXIA', 't_dashboard_task_failed_today');
*/
