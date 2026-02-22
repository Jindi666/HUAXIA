-- ================================================================
-- 创建 uibot_rpa 数据库和表的 SQL 脚本
-- 用途：支持华夏基金大屏项目的跨库查询
-- 作者：Claude Code
-- 日期：2026-02-04
-- ================================================================

-- 1. 创建数据库
CREATE DATABASE IF NOT EXISTS uibot_rpa
DEFAULT CHARACTER SET utf8mb4
DEFAULT COLLATE utf8mb4_unicode_ci;

USE uibot_rpa;

-- ================================================================
-- 2. 创建表结构
-- ================================================================

-- 表 1: tbl_user_worker (人机交互机器人表)
-- 对应查询：
--   SELECT COUNT(*) FROM uibot_rpa.tbl_user_worker WHERE worker_state=2  (在线)
--   SELECT COUNT(*) FROM uibot_rpa.tbl_user_worker WHERE worker_state=3  (离线)
CREATE TABLE IF NOT EXISTS tbl_user_worker (
    worker_id BIGINT PRIMARY KEY AUTO_INCREMENT COMMENT '机器人ID',
    worker_name VARCHAR(100) NOT NULL COMMENT '机器人名称',
    worker_state TINYINT NOT NULL DEFAULT 3 COMMENT '机器人状态：1=未知, 2=在线, 3=离线',
    worker_type TINYINT NOT NULL DEFAULT 1 COMMENT '机器人类型：1=人机交互, 2=无人值守',
    dep_id VARCHAR(50) COMMENT '部门ID',
    dep_name VARCHAR(100) COMMENT '部门名称',
    user_id VARCHAR(50) COMMENT '用户ID',
    user_name VARCHAR(100) COMMENT '用户名称',
    ip_address VARCHAR(50) COMMENT 'IP地址',
    last_heartbeat DATETIME COMMENT '最后心跳时间',
    create_time DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_worker_state (worker_state),
    INDEX idx_worker_type (worker_type),
    INDEX idx_dep_id (dep_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='人机交互机器人表';

-- 表 2: tbl_worker (机器人表 - 包含无人值守)
-- 对应查询：
--   SELECT COUNT(*) FROM uibot_rpa.tbl_worker WHERE worker_type=2 AND worker_state=2  (无人值守在线)
--   SELECT COUNT(*) FROM uibot_rpa.tbl_worker WHERE worker_type=2 AND worker_state=3  (无人值守离线)
CREATE TABLE IF NOT EXISTS tbl_worker (
    worker_id BIGINT PRIMARY KEY AUTO_INCREMENT COMMENT '机器人ID',
    worker_name VARCHAR(100) NOT NULL COMMENT '机器人名称',
    worker_state TINYINT NOT NULL DEFAULT 3 COMMENT '机器人状态：1=未知, 2=在线, 3=离线',
    worker_type TINYINT NOT NULL DEFAULT 2 COMMENT '机器人类型：1=人机交互, 2=无人值守',
    dep_id VARCHAR(50) COMMENT '部门ID',
    dep_name VARCHAR(100) COMMENT '部门名称',
    ip_address VARCHAR(50) COMMENT 'IP地址',
    last_heartbeat DATETIME COMMENT '最后心跳时间',
    create_time DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_worker_state (worker_state),
    INDEX idx_worker_type (worker_type),
    INDEX idx_dep_id (dep_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='机器人表';

-- ================================================================
-- 3. 插入测试数据
-- ================================================================

-- 插入人机交互机器人测试数据
INSERT INTO tbl_user_worker (worker_name, worker_state, worker_type, dep_id, dep_name, user_id, user_name, ip_address, last_heartbeat) VALUES
('人机交互机器人-财务部-01', 2, 1, 'DEPT001', '财务部', 'USER001', '张三', '192.168.1.101', NOW()),
('人机交互机器人-财务部-02', 2, 1, 'DEPT001', '财务部', 'USER002', '李四', '192.168.1.102', NOW()),
('人机交互机器人-财务部-03', 3, 1, 'DEPT001', '财务部', 'USER003', '王五', '192.168.1.103', DATE_SUB(NOW(), INTERVAL 2 HOUR)),
('人机交互机器人-IT部-01', 2, 1, 'DEPT002', 'IT部', 'USER004', '赵六', '192.168.1.201', NOW()),
('人机交互机器人-IT部-02', 3, 1, 'DEPT002', 'IT部', 'USER005', '钱七', '192.168.1.202', DATE_SUB(NOW(), INTERVAL 1 HOUR)),
('人机交互机器人-人力资源-01', 2, 1, 'DEPT003', '人力资源部', 'USER006', '孙八', '192.168.1.301', NOW()),
('人机交互机器人-人力资源-02', 2, 1, 'DEPT003', '人力资源部', 'USER007', '周九', '192.168.1.302', NOW()),
('人机交互机器人-人力资源-03', 3, 1, 'DEPT003', '人力资源部', 'USER008', '吴十', '192.168.1.303', DATE_SUB(NOW(), INTERVAL 3 HOUR)),
('人机交互机器人-运营部-01', 2, 1, 'DEPT004', '运营部', 'USER009', '郑十一', '192.168.1.401', NOW()),
('人机交互机器人-运营部-02', 3, 1, 'DEPT004', '运营部', 'USER010', '王十二', '192.168.1.402', DATE_SUB(NOW(), INTERVAL 4 HOUR));

-- 插入无人值守机器人测试数据
INSERT INTO tbl_worker (worker_name, worker_state, worker_type, dep_id, dep_name, ip_address, last_heartbeat) VALUES
('无人值守机器人-数据处理-01', 2, 2, 'DEPT001', '财务部', '192.168.2.101', NOW()),
('无人值守机器人-数据处理-02', 2, 2, 'DEPT001', '财务部', '192.168.2.102', NOW()),
('无人值守机器人-数据处理-03', 3, 2, 'DEPT001', '财务部', '192.168.2.103', DATE_SUB(NOW(), INTERVAL 5 HOUR)),
('无人值守机器人-报表生成-01', 2, 2, 'DEPT002', 'IT部', '192.168.2.201', NOW()),
('无人值守机器人-报表生成-02', 3, 2, 'DEPT002', 'IT部', '192.168.2.202', DATE_SUB(NOW(), INTERVAL 2 HOUR)),
('无人值守机器人-报表生成-03', 3, 2, 'DEPT002', 'IT部', '192.168.2.203', DATE_SUB(NOW(), INTERVAL 6 HOUR)),
('无人值守机器人-数据同步-01', 2, 2, 'DEPT003', '人力资源部', '192.168.2.301', NOW()),
('无人值守机器人-数据同步-02', 2, 2, 'DEPT003', '人力资源部', '192.168.2.302', NOW()),
('无人值守机器人-邮件发送-01', 3, 2, 'DEPT004', '运营部', '192.168.2.401', DATE_SUB(NOW(), INTERVAL 8 HOUR)),
('无人值守机器人-邮件发送-02', 2, 2, 'DEPT004', '运营部', '192.168.2.402', NOW()),
('无人值守机器人-文件归档-01', 2, 2, 'DEPT005', '行政部', '192.168.2.501', NOW()),
('无人值守机器人-文件归档-02', 3, 2, 'DEPT005', '行政部', '192.168.2.502', DATE_SUB(NOW(), INTERVAL 10 HOUR));

-- ================================================================
-- 4. 验证查询
-- ================================================================

-- 验证表是否创建成功
SELECT
    '数据库' AS category,
    DATABASE() AS name
UNION ALL
SELECT
    '表' AS category,
    TABLE_NAME AS name
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = 'uibot_rpa'
ORDER BY category;

-- 验证跨库查询 - 人机交互机器人在线数量
SELECT
    '人机交互-在线' AS type,
    COUNT(*) AS count
FROM uibot_rpa.tbl_user_worker
WHERE worker_state = 2;

-- 验证跨库查询 - 人机交互机器人离线数量
SELECT
    '人机交互-离线' AS type,
    COUNT(*) AS count
FROM uibot_rpa.tbl_user_worker
WHERE worker_state = 3;

-- 验证跨库查询 - 无人值守机器人在线数量
SELECT
    '无人值守-在线' AS type,
    COUNT(*) AS count
FROM uibot_rpa.tbl_worker
WHERE worker_type = 2 AND worker_state = 2;

-- 验证跨库查询 - 无人值守机器人离线数量
SELECT
    '无人值守-离线' AS type,
    COUNT(*) AS count
FROM uibot_rpa.tbl_worker
WHERE worker_type = 2 AND worker_state = 3;

-- 统计摘要
SELECT
    '人机交互-在线' AS description,
    (SELECT COUNT(*) FROM uibot_rpa.tbl_user_worker WHERE worker_state=2) AS count
UNION ALL
SELECT
    '人机交互-离线' AS description,
    (SELECT COUNT(*) FROM uibot_rpa.tbl_user_worker WHERE worker_state=3) AS count
UNION ALL
SELECT
    '无人值守-在线' AS description,
    (SELECT COUNT(*) FROM uibot_rpa.tbl_worker WHERE worker_type=2 AND worker_state=2) AS count
UNION ALL
SELECT
    '无人值守-离线' AS description,
    (SELECT COUNT(*) FROM uibot_rpa.tbl_worker WHERE worker_type=2 AND worker_state=3) AS count;

-- ================================================================
-- 5. 权限配置（如果需要）
-- ================================================================

-- 确保 root 用户有权限访问
-- 通常 root 用户已经拥有所有权限，但如果遇到权限问题，可以执行：
-- GRANT ALL PRIVILEGES ON uibot_rpa.* TO 'root'@'localhost';
-- FLUSH PRIVILEGES;

-- ================================================================
-- 完成！
-- ================================================================
SELECT '✅ uibot_rpa 数据库创建成功！可以开始跨库查询了。' AS message;
