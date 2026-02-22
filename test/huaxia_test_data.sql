-- =====================================================
-- 华夏基金大屏监控 - 测试数据插入脚本
-- 数据库: 达梦数据库 (Dameng)
-- Schema: HUAXIA
-- 日期: 2026-02-06
-- =====================================================

-- 设置当前 Schema
SET SCHEMA HUAXIA;

-- =====================================================
-- 1. 顶部监控信息表 (t_dashboard_monitor_topinfo)
-- 用途: 本月节省工时、流程总数、任务成功数、任务总数
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_topinfo (updateDate, totalTaskCount, successTaskCount, totalFlowCount, saveWorkHour)
VALUES (
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),  -- updateDate
    15234,                                -- 本月任务总数
    14856,                                -- 本月任务成功数
    128,                                  -- 本月新增流程数
    3245.8                                -- 本月节省工时(小时)
);

-- =====================================================
-- 2. 今日任务失败统计表 (t_dashboard_task_failed_today)
-- 用途: 今日失败任务数和今日任务总数
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_task_failed_today (updateDate, failedTaskCountToday, totalTaskCountToday)
VALUES (
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),  -- updateDate
    23,                                   -- 今日失败任务数
    542                                   -- 今日任务总数
);

-- =====================================================
-- 3. 今日失败任务详情表 (t_dashboard_monitor_today_taskfailed_info)
-- 用途: 今日失败任务详情列表
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_today_taskfailed_info
(queryDate, updateDate, updateTime, deptName, workerName, flowName, content)
VALUES
(
    TO_DATE('2026-02-06 10:23:45', 'YYYY-MM-DD HH24:MI:SS'),  -- queryDate
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),                      -- updateDate
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),  -- updateTime
    'IT技术部',                                                   -- 部门
    'RPA_Worker_001',                                           -- worker名称
    '数据同步流程',                                               -- 流程名称
    '连接数据库超时,错误码: ERR-DB-001,无法连接到目标数据库服务器'    -- 报错信息
),
(
    TO_DATE('2026-02-06 11:15:22', 'YYYY-MM-DD HH24:MI:SS'),
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),
    '财务部',
    'RPA_Worker_002',
    '发票识别流程',
    'OCR识别失败,错误码: ERR-OCR-002,图片格式不支持或图片质量过低'
),
(
    TO_DATE('2026-02-06 13:45:10', 'YYYY-MM-DD HH24:MI:SS'),
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),
    '人力资源部',
    'RPA_Worker_003',
    '员工入职流程',
    '系统API调用失败,错误码: ERR-API-403,权限不足或Token过期'
),
(
    TO_DATE('2026-02-06 09:30:15', 'YYYY-MM-DD HH24:MI:SS'),
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),
    '运营部',
    'RPA_Worker_004',
    '数据报表生成',
    '内存溢出错误,错误码: ERR-MEM-005,处理数据量过大导致内存不足'
),
(
    TO_DATE('2026-02-06 14:20:33', 'YYYY-MM-DD HH24:MI:SS'),
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),
    'IT技术部',
    'RPA_Worker_001',
    '系统巡检流程',
    '网络连接中断,错误码: ERR-NET-006,无法访问目标服务器'
);

-- =====================================================
-- 4. 近3个月流程数量统计表 (t_dashboard_monitor_3month_flowcount)
-- 用途: 按天统计流程数量
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_flowcount (queryDate, flowCount)
VALUES
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 95),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 102),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 98),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 105),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 112),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 128),
(TO_DATE('2026-02-07', 'YYYY-MM-DD'), 115),
(TO_DATE('2026-02-08', 'YYYY-MM-DD'), 108);

-- =====================================================
-- 5. 近3个月任务数量统计表 (t_dashboard_monitor_3month_taskcount)
-- 用途: 按天统计任务数量
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_taskcount (queryDate, taskCount)
VALUES
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 485),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 512),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 498),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 525),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 538),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 542),
(TO_DATE('2026-02-07', 'YYYY-MM-DD'), 520),
(TO_DATE('2026-02-08', 'YYYY-MM-DD'), 508);

-- =====================================================
-- 6. 近3个月任务成功率统计表 (t_dashboard_monitor_3month_taskrate)
-- 用途: 按天统计任务成功率
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_taskrate (queryDate, taskRate)
VALUES
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 95.8),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 96.2),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 94.5),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 97.1),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 96.8),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 95.8),
(TO_DATE('2026-02-07', 'YYYY-MM-DD'), 97.5),
(TO_DATE('2026-02-08', 'YYYY-MM-DD'), 96.9);

-- =====================================================
-- 7. 近3个月新增流程统计表 (t_dashboard_monitor_3month_flowaddedinfo)
-- 用途: 按周统计新增流程
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_flowaddedinfo (weekName, flowCount, sortflag)
VALUES
('第1周 (11月6日-11月12日)', 12, 1),
('第2周 (11月13日-11月19日)', 15, 2),
('第3周 (11月20日-11月26日)', 18, 3),
('第4周 (11月27日-12月3日)', 22, 4),
('第5周 (12月4日-12月10日)', 20, 5),
('第6周 (12月11日-12月17日)', 25, 6),
('第7周 (12月18日-12月24日)', 28, 7),
('第8周 (12月25日-12月31日)', 32, 8),
('第9周 (1月1日-1月7日)', 30, 9),
('第10周 (1月8日-1月14日)', 35, 10),
('第11周 (1月15日-1月21日)', 38, 11),
('第12周 (1月22日-1月28日)', 42, 12);

-- =====================================================
-- 8. 实时任务监控表 (t_dashboard_monitor_realtime_info)
-- 用途: 实时监控正在运行的任务
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_realtime_info
(startTime, updateDate, updateTime, deptName, workerName, flowName, taskId, taskState)
VALUES
(TO_DATE('2026-02-06 14:25:33', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), 'IT技术部', 'RPA_Worker_001', '数据同步流程', 'TASK-20260206-001', '运行中'),
(TO_DATE('2026-02-06 14:26:15', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '财务部', 'RPA_Worker_002', '发票识别流程', 'TASK-20260206-002', '运行中'),
(TO_DATE('2026-02-06 14:27:42', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '人力资源部', 'RPA_Worker_003', '员工入职流程', 'TASK-20260206-003', '已完成'),
(TO_DATE('2026-02-06 14:28:10', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '运营部', 'RPA_Worker_004', '数据报表生成', 'TASK-20260206-004', '运行中'),
(TO_DATE('2026-02-06 14:29:25', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), 'IT技术部', 'RPA_Worker_005', '系统巡检流程', 'TASK-20260206-005', '待运行'),
(TO_DATE('2026-02-06 14:20:18', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '财务部', 'RPA_Worker_006', '财务对账流程', 'TASK-20260206-006', '已完成'),
(TO_DATE('2026-02-06 14:21:45', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '人力资源部', 'RPA_Worker_007', '薪酬计算流程', 'TASK-20260206-007', '运行中'),
(TO_DATE('2026-02-06 14:22:33', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '运营部', 'RPA_Worker_008', '数据导出流程', 'TASK-20260206-008', '运行中'),
(TO_DATE('2026-02-06 14:23:50', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), 'IT技术部', 'RPA_Worker_009', '备份流程', 'TASK-20260206-009', '已完成'),
(TO_DATE('2026-02-06 14:24:12', 'YYYY-MM-DD HH24:MI:SS'), TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '财务部', 'RPA_Worker_010', '凭证生成流程', 'TASK-20260206-010', '失败');

-- =====================================================
-- 9. 实时部门任务信息表 (t_dashboard_monitor_realtime_depttaskinfo)
-- 用途: 按部门统计任务运行情况
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_realtime_depttaskinfo
(updateDate, updateTime, deptName, taskSuccess, taskFail, taskRunning, taskDeploying)
VALUES
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), 'IT技术部', 156, 8, 12, 5),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '财务部', 142, 12, 8, 3),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '人力资源部', 98, 5, 6, 2),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '运营部', 124, 10, 9, 4),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '市场部', 85, 3, 4, 2);

-- =====================================================
-- 10. 任务失败Top5统计表 (t_dashboard_monitor_top5_taskfailedcount)
-- 用途: 统计失败次数最多的流程
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_top5_taskfailedcount
(updateDate, mouth, deptName, flowName, taskFailedCount, queryTime)
VALUES
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, 'IT技术部', '数据同步流程', 15, '2026-02-06 10:23:45,2026-02-05 15:30:22,2026-02-04 09:15:10,2026-02-03 14:20:33,2026-02-02 11:25:18,2026-02-01 16:40:55'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '财务部', '发票识别流程', 12, '2026-02-06 11:15:22,2026-02-05 10:45:30,2026-02-04 14:20:15,2026-02-03 09:30:45,2026-02-02 15:10:20,2026-02-01 12:25:33'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '运营部', '数据报表生成', 10, '2026-02-06 09:30:15,2026-02-05 13:20:40,2026-02-04 11:15:25,2026-02-03 16:40:10,2026-02-02 10:25:50,2026-02-01 14:30:15'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '人力资源部', '员工入职流程', 8, '2026-02-06 13:45:10,2026-02-05 09:20:30,2026-02-04 15:35:15,2026-02-03 11:40:20,2026-02-02 14:15:45,2026-02-01 10:50:25'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, 'IT技术部', '系统巡检流程', 6, '2026-02-06 14:20:33,2026-02-05 10:15:20,2026-02-04 13:30:45,2026-02-03 09:45:30,2026-02-02 15:20:15,2026-02-01 11:35:40');

-- =====================================================
-- 11. 任务失败原因统计表 (t_dashboard_monitor_failedReason_discount)
-- 用途: 统计各类失败原因及次数
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_failedReason_discount
(updateDate, errcode, deptName, flowName, errcodeCount)
VALUES
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-DB-001,ERR-DB-002,ERR-DB-003', 'IT技术部', '数据同步流程', 18),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-OCR-002,ERR-OCR-005', '财务部', '发票识别流程', 15),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-MEM-005', '运营部', '数据报表生成', 12),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-API-403,ERR-API-401', '人力资源部', '员工入职流程', 10),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-NET-006,ERR-NET-008', 'IT技术部', '系统巡检流程', 8),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-TIMEOUT-007', '财务部', '财务对账流程', 6),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'ERR-FILE-004', '运营部', '文件处理流程', 5);

-- =====================================================
-- 12. 近7天部门失败率统计表 (t_dashboard_monitor_7days_dept_failedrate)
-- 用途: 按部门统计近7天任务失败率
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_7days_dept_failedrate
(queryDate, deptName, taskFailedRate)
VALUES
-- IT技术部
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), 'IT技术部', 5.2),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 'IT技术部', 4.8),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 'IT技术部', 5.5),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 'IT技术部', 4.2),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 'IT技术部', 3.9),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 'IT技术部', 4.5),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'IT技术部', 4.8),
-- 财务部
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), '财务部', 6.8),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), '财务部', 7.2),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), '财务部', 6.5),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), '财务部', 5.8),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), '财务部', 6.2),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), '财务部', 5.9),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), '财务部', 6.5),
-- 人力资源部
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), '人力资源部', 3.2),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), '人力资源部', 2.8),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), '人力资源部', 3.5),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), '人力资源部', 3.0),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), '人力资源部', 2.5),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), '人力资源部', 2.9),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), '人力资源部', 3.1),
-- 运营部
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), '运营部', 5.5),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), '运营部', 5.0),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), '运营部', 5.8),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), '运营部', 4.8),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), '运营部', 5.2),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), '运营部', 4.9),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), '运营部', 5.3);

-- =====================================================
-- 13. 近7天机器人失败率统计表 (t_dashboard_monitor_7days_worker_failedrate)
-- 用途: 按机器人统计近7天任务失败率
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_7days_worker_failedrate
(queryDate, workerName, workerFailedRate)
VALUES
-- RPA_Worker_001
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), 'RPA_Worker_001', 4.5),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 'RPA_Worker_001', 4.2),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 'RPA_Worker_001', 4.8),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 'RPA_Worker_001', 3.9),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 'RPA_Worker_001', 3.5),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 'RPA_Worker_001', 4.0),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'RPA_Worker_001', 4.3),
-- RPA_Worker_002
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), 'RPA_Worker_002', 6.2),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 'RPA_Worker_002', 5.8),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 'RPA_Worker_002', 6.5),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 'RPA_Worker_002', 5.5),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 'RPA_Worker_002', 5.2),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 'RPA_Worker_002', 5.9),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'RPA_Worker_002', 6.0),
-- RPA_Worker_003
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.8),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.5),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 'RPA_Worker_003', 3.0),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.2),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.0),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.4),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'RPA_Worker_003', 2.6),
-- RPA_Worker_004
(TO_DATE('2026-01-31', 'YYYY-MM-DD'), 'RPA_Worker_004', 5.0),
(TO_DATE('2026-02-01', 'YYYY-MM-DD'), 'RPA_Worker_004', 4.5),
(TO_DATE('2026-02-02', 'YYYY-MM-DD'), 'RPA_Worker_004', 5.3),
(TO_DATE('2026-02-03', 'YYYY-MM-DD'), 'RPA_Worker_004', 4.2),
(TO_DATE('2026-02-04', 'YYYY-MM-DD'), 'RPA_Worker_004', 4.8),
(TO_DATE('2026-02-05', 'YYYY-MM-DD'), 'RPA_Worker_004', 4.6),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 'RPA_Worker_004', 4.9);

COMMIT;

-- =====================================================
-- 数据验证查询
-- =====================================================
-- 查询顶部监控信息
-- SELECT * FROM HUAXIA.t_dashboard_monitor_topinfo WHERE updateDate = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询今日失败任务统计
-- SELECT * FROM HUAXIA.t_dashboard_task_failed_today WHERE updateDate = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询实时任务监控
-- SELECT * FROM HUAXIA.t_dashboard_monitor_realtime_info WHERE updateDate = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询部门任务统计
-- SELECT * FROM HUAXIA.t_dashboard_monitor_realtime_depttaskinfo WHERE updateDate = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- =====================================================
-- 说明
-- =====================================================
-- 1. 本脚本包含13张表的测试数据
-- 2. 所有日期使用2026-02-06作为当前日期
-- 3. 数据涵盖IT技术部、财务部、人力资源部、运营部、市场部5个部门
-- 4. 包含10个RPA机器人的运行数据
-- 5. 数据包括成功、失败、运行中、待运行等多种状态
-- 6. 达梦数据库兼容MySQL语法,使用TO_DATE函数进行日期转换
-- 7. 所有字段名使用驼峰命名(camelCase),如: updateDate, totalTaskCount, deptName等
-- =====================================================
