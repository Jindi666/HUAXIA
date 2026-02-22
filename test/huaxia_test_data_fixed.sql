-- =====================================================
-- 华夏基金大屏监控 - 测试数据插入脚本 (修正版)
-- 数据库: 达梦数据库 (Dameng)
-- Schema: HUAXIA
-- 日期: 2026-02-06
-- 说明: 根据后端实际SQL查询的字段名修正
-- =====================================================

-- 设置当前 Schema
SET SCHEMA HUAXIA;

-- =====================================================
-- 1. 顶部监控信息表 (t_dashboard_monitor_topinfo)
-- 实际字段: update_date, totaltaskcount, successtaskcount, totalflowcount, saveworkhour
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_topinfo (update_date, totaltaskcount, successtaskcount, totalflowcount, saveworkhour)
VALUES (
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),  -- update_date
    15234,                                -- 本月任务总数
    14856,                                -- 本月任务成功数
    128,                                  -- 本月新增流程数
    3245.8                                -- 本月节省工时(小时)
);

-- =====================================================
-- 2. 今日任务失败统计表 (t_dashboard_task_failed_today)
-- 实际字段: update_date, FailedTaskCountToday, TotalTaskCountToday
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_task_failed_today (update_date, FailedTaskCountToday, TotalTaskCountToday)
VALUES (
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),  -- update_date
    23,                                   -- 今日失败任务数
    542                                   -- 今日任务总数
);

-- =====================================================
-- 3. 今日失败任务详情表 (t_dashboard_monitor_today_taskfailed_info)
-- 实际字段: query_date, update_date, update_time, dep_name, worker_name, flow_name, content
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_today_taskfailed_info
(query_date, update_date, update_time, dep_name, worker_name, flow_name, content)
VALUES
(
    TO_DATE('2026-02-06 10:23:45', 'YYYY-MM-DD HH24:MI:SS'),  -- query_date
    TO_DATE('2026-02-06', 'YYYY-MM-DD'),                      -- update_date
    TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'),  -- update_time
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
-- 实际字段: query_date, flow_count
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_flowcount (query_date, flow_count)
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
-- 实际字段: query_date, task_count
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_taskcount (query_date, task_count)
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
-- 实际字段: query_date, task_rate
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_taskrate (query_date, task_rate)
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
-- 实际字段: weekname, flow_count, sortflag
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_3month_flowaddedinfo (weekname, flow_count, sortflag)
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
-- 实际字段: start_time, update_date, update_time, dep_name, worker_name, flow_name, task_id, task_state
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_realtime_info
(start_time, update_date, update_time, dep_name, worker_name, flow_name, task_id, task_state)
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
-- 实际字段: update_date, update_time, dep_name, task_success, task_failed, task_running, task_deploying
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_realtime_depttaskinfo
(update_date, update_time, dep_name, task_success, task_failed, task_running, task_deploying)
VALUES
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), 'IT技术部', 156, 8, 12, 5),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '财务部', 142, 12, 8, 3),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '人力资源部', 98, 5, 6, 2),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '运营部', 124, 10, 9, 4),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), TO_DATE('2026-02-06 14:30:00', 'YYYY-MM-DD HH24:MI:SS'), '市场部', 85, 3, 4, 2);

-- =====================================================
-- 10. 任务失败Top5统计表 (t_dashboard_monitor_top5_taskfailedcount)
-- 实际字段: update_date, mouth, dep_name, flow_name, task_failed_count, query_time
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_top5_taskfailedcount
(update_date, mouth, dep_name, flow_name, task_failed_count, query_time)
VALUES
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, 'IT技术部', '数据同步流程', 15, '2026-02-06 10:23:45,2026-02-05 15:30:22,2026-02-04 09:15:10,2026-02-03 14:20:33,2026-02-02 11:25:18,2026-02-01 16:40:55'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '财务部', '发票识别流程', 12, '2026-02-06 11:15:22,2026-02-05 10:45:30,2026-02-04 14:20:15,2026-02-03 09:30:45,2026-02-02 15:10:20,2026-02-01 12:25:33'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '运营部', '数据报表生成', 10, '2026-02-06 09:30:15,2026-02-05 13:20:40,2026-02-04 11:15:25,2026-02-03 16:40:10,2026-02-02 10:25:50,2026-02-01 14:30:15'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, '人力资源部', '员工入职流程', 8, '2026-02-06 13:45:10,2026-02-05 09:20:30,2026-02-04 15:35:15,2026-02-03 11:40:20,2026-02-02 14:15:45,2026-02-01 10:50:25'),
(TO_DATE('2026-02-06', 'YYYY-MM-DD'), 3, 'IT技术部', '系统巡检流程', 6, '2026-02-06 14:20:33,2026-02-05 10:15:20,2026-02-04 13:30:45,2026-02-03 09:45:30,2026-02-02 15:20:15,2026-02-01 11:35:40');

-- =====================================================
-- 11. 任务失败原因统计表 (t_dashboard_monitor_failedreason_discount)
-- 实际字段: update_date, errcode, dep_name, flow_name, errcode_count
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_failedreason_discount
(update_date, errcode, dep_name, flow_name, errcode_count)
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
-- 实际字段: query_date, dep_name, task_failed_rate
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_7days_dept_failedrate
(query_date, dep_name, task_failed_rate)
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
-- 实际字段: query_date, worker_name, worker_failedrate
-- =====================================================
INSERT INTO HUAXIA.t_dashboard_monitor_7days_worker_failedrate
(query_date, worker_name, worker_failedrate)
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
-- SELECT * FROM HUAXIA.t_dashboard_monitor_topinfo WHERE update_date = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询今日失败任务统计
-- SELECT * FROM HUAXIA.t_dashboard_task_failed_today WHERE update_date = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询实时任务监控
-- SELECT * FROM HUAXIA.t_dashboard_monitor_realtime_info WHERE update_date = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- 查询部门任务统计
-- SELECT * FROM HUAXIA.t_dashboard_monitor_realtime_depttaskinfo WHERE update_date = TO_DATE('2026-02-06', 'YYYY-MM-DD');

-- =====================================================
-- 说明
-- =====================================================
-- 1. 本脚本包含13张表的测试数据
-- 2. 所有日期使用2026-02-06作为当前日期
-- 3. 数据涵盖IT技术部、财务部、人力资源部、运营部、市场部5个部门
-- 4. 包含10个RPA机器人的运行数据
-- 5. 数据包括成功、失败、运行中、待运行等多种状态
-- 6. 达梦数据库兼容MySQL语法,使用TO_DATE函数进行日期转换
-- 7. 所有字段名根据后端实际SQL查询日志修正,使用小写+下划线命名
-- 8. 字段名示例: update_date, dep_name, worker_name, flow_name, task_success等
-- =====================================================
