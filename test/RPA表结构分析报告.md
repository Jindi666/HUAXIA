# RPA数据库表结构分析报告

## 📋 发现的问题

### 原MySQL代码中的查询

```sql
-- HuaxiajijinController.cs 第342行
select count(*) as manMachineOnline
from uibot_rpa.tbl_user_worker
where worker_state=2

-- 第348行
select count(*) as manMachineNotOnline
from uibot_rpa.tbl_user_worker
where worker_state=3

-- 第354行
select count(*) as unmannedOnline
from uibot_rpa.tbl_worker
where worker_type=2 and worker_state=2

-- 第360行
select count(*) as unmannedNotOnline
from uibot_rpa.tbl_worker
where worker_type=2 and worker_state=3
```

### 达梦数据库中实际的表结构

**表名映射**:
- `uibot_rpa.tbl_user_worker` → `RPA.TBL_CMD_ATTENDED_WORKER`
- `uibot_rpa.tbl_worker` → `RPA.TBL_CMD_WORKER`

#### TBL_CMD_ATTENDED_WORKER 表结构 (人机交互工人)

| 列名 | 数据类型 | 说明 |
|------|---------|------|
| ID | BIGINT | 主键 |
| EMPLOYEE_ID | BIGINT | 员工ID |
| EMPLOYEE_NAME | VARCHAR | 员工姓名 |
| COMPANY_ID | BIGINT | 公司ID |
| MACHINE_NAME | VARCHAR | 机器名称 |
| MACHINE_CODE | VARCHAR | 机器编码 |
| WORKER_VERSION | VARCHAR | Worker版本 |
| **IS_ONLINE** | **SMALLINT** | **是否在线(此表的关键字段!)** |
| LAST_LOGIN_TIME | TIMESTAMP | 最后登录时间 |
| LAST_LOGIN_IP | VARCHAR | 最后登录IP |
| LAST_HEARTBEAT_TIME | TIMESTAMP | 最后心跳时间 |

**问题**: ❌ **此表没有 `WORKER_STATE` 列!**

#### TBL_CMD_WORKER 表结构 (无人值守工人)

| 列名 | 数据类型 | 说明 |
|------|---------|------|
| ID | BIGINT | 主键 |
| WORKER_ID | BIGINT | Worker ID |
| WORKER_NAME | VARCHAR | Worker名称 |
| WORKER_TYPE | INT | Worker类型 |
| **WORKER_STATE** | **INT** | **Worker状态(此表有关键字段!)** |
| **IS_ONLINE** | **SMALLINT** | 是否在线 |
| LAST_HEARTBEAT_TIME | TIMESTAMP | 最后心跳时间 |
| ... | ... | 其他字段 |

**说明**: ✅ 此表有 `WORKER_STATE` 和 `WORKER_TYPE` 列

---

## 🎯 问题分析

### MySQL到达梦数据库迁移中的差异

1. **表名不同**:
   - MySQL: `tbl_user_worker` → 达梦: `TBL_CMD_ATTENDED_WORKER`
   - MySQL: `tbl_worker` → 达梦: `TBL_CMD_WORKER` (名称一致)

2. **字段不同**:
   - MySQL中的 `tbl_user_worker` 表有 `worker_state` 字段
   - 达梦中的 `TBL_CMD_ATTENDED_WORKER` 表**没有** `worker_state` 字段,只有 `IS_ONLINE` 字段

3. **数据状态**:
   - `RPA.TBL_CMD_ATTENDED_WORKER` 表: **0条记录**
   - `RPA.TBL_CMD_WORKER` 表: 需要查询确认

---

## 💡 解决方案

### 方案1: 修改查询逻辑 (推荐)

由于达梦数据库的表结构不同,需要修改查询逻辑:

```sql
-- 原MySQL查询(错误)
select count(*) as manMachineOnline
from uibot_rpa.tbl_user_worker
where worker_state=2

-- 修改为达梦查询(正确)
select count(*) as manMachineOnline
from RPA.TBL_CMD_ATTENDED_WORKER
where IS_ONLINE = 1

-- 原MySQL查询(错误)
select count(*) as manMachineNotOnline
from uibot_rpa.tbl_user_worker
where worker_state=3

-- 修改为达梦查询(正确)
select count(*) as manMachineNotOnline
from RPA.TBL_CMD_ATTENDED_WORKER
where IS_ONLINE = 0 OR IS_ONLINE IS NULL

-- 无人值守工人查询保持不变
select count(*) as unmannedOnline
from RPA.TBL_CMD_WORKER
where WORKER_TYPE = 2 AND WORKER_STATE = 2
```

### 方案2: 在达梦中添加WORKER_STATE字段

如果需要保持与MySQL完全一致,可以在`TBL_CMD_ATTENDED_WORKER`表中添加`WORKER_STATE`字段:

```sql
ALTER TABLE RPA.TBL_CMD_ATTENDED_WORKER ADD WORKER_STATE INT;

-- 根据IS_ONLINE字段更新WORKER_STATE
UPDATE RPA.TBL_CMD_ATTENDED_WORKER
SET WORKER_STATE = CASE
    WHEN IS_ONLINE = 1 THEN 2  -- 在线
    WHEN IS_ONLINE = 0 THEN 3  -- 离线
    ELSE 3                     -- 默认离线
END;
```

### 方案3: 创建视图兼容

创建一个视图来兼容MySQL的表结构:

```sql
CREATE OR REPLACE VIEW RPA.V_USER_WORKER AS
SELECT
    ID,
    EMPLOYEE_ID,
    EMPLOYEE_NAME,
    COMPANY_ID,
    MACHINE_NAME,
    WORKER_VERSION,
    CASE
        WHEN IS_ONLINE = 1 THEN 2
        WHEN IS_ONLINE = 0 THEN 3
        ELSE 3
    END AS WORKER_STATE,
    LAST_LOGIN_TIME,
    LAST_LOGIN_IP,
    LAST_HEARTBEAT_TIME
FROM RPA.TBL_CMD_ATTENDED_WORKER;
```

然后在代码中查询`RPA.V_USER_WORKER`视图。

---

## 📝 建议的代码修改

### 修改HuaxiajijinController.cs

```csharp
[HttpPost("workerOnline")]
public BaseResponse<WorkerBean> WorkerOnline([FromServices] IFreeSql conn)
{
    try
    {
        // 人机交互在线 - 使用IS_ONLINE字段
        StringBuilder sb1 = new StringBuilder();
        sb1.append(" SELECT COUNT(*) as manMachineOnline FROM RPA.TBL_CMD_ATTENDED_WORKER WHERE IS_ONLINE = 1 ");
        var sql1 = sb1.toString();
        var manMachineBean = conn.Select<ManOnlineBean>().WithSql(@sql1).ToOne();

        // 人机交互离线 - 使用IS_ONLINE字段
        StringBuilder sb2 = new StringBuilder();
        sb2.append(" SELECT COUNT(*) as manMachineNotOnline FROM RPA.TBL_CMD_ATTENDED_WORKER WHERE (IS_ONLINE = 0 OR IS_ONLINE IS NULL) ");
        var sql2 = sb2.toString();
        var manNotOnlineBean = conn.Select<ManNotOnlineBean>().WithSql(@sql2).ToOne();

        // 无人值守在线 - 保持不变
        StringBuilder sb3 = new StringBuilder();
        sb3.append(" SELECT COUNT(*) as unmannedOnline FROM RPA.TBL_CMD_WORKER WHERE WORKER_TYPE = 2 AND WORKER_STATE = 2 ");
        var sql3 = sb3.toString();
        var unManOnlineBean = conn.Select<UnManOnlineBean>().WithSql(@sql3).ToOne();

        // 无人值守离线 - 保持不变
        StringBuilder sb4 = new StringBuilder();
        sb4.append(" SELECT COUNT(*) as unmannedNotOnline FROM RPA.TBL_CMD_WORKER WHERE WORKER_TYPE = 2 AND WORKER_STATE = 3 ");
        var sql4 = sb4.toString();
        var unManNotOnlineBean = conn.Select<UnManNotOnlineBean>().WithSql(@sql4).ToOne();

        // ... 其余代码保持不变
    }
    catch (Exception ex)
    {
        // ... 错误处理
    }
}
```

---

## ⚠️ 注意事项

1. **字段映射关系**:
   - MySQL的 `worker_state=2` (在线) → 达梦的 `IS_ONLINE=1`
   - MySQL的 `worker_state=3` (离线) → 达梦的 `IS_ONLINE=0`

2. **NULL值处理**:
   - 达梦数据库中 `IS_ONLINE` 可能为 NULL,需要使用 `IS_ONLINE IS NULL` 来判断

3. **数据量**:
   - 目前 `TBL_CMD_ATTENDED_WORKER` 表中**没有数据**(0条记录)
   - 需要先确认是否有测试数据,或者生产环境是否有数据

---

**报告生成时间**: 2026-02-05
**测试人员**: Claude Code
**项目名称**: 华夏基金大屏后端 - laiye-customer-webapi-feature-huaxiajijin
