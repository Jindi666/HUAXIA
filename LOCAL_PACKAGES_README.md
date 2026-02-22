# 本地包配置说明

## 配置概述

由于项目依赖的来也科技私有 NuGet 包无法从公共源获取，已从 Docker 镜像中提取并配置为本地引用。

## 所做的修改

### 1. 创建的文件和目录

```
laiye-customer-webapi-feature-huaxiajijin/
├── local-packages/                          # 新建：本地包文件夹
│   ├── Laiye.EntCmd.Service.Grpc.dll       # 2.1 MB - 命令中心 gRPC 客户端
│   ├── Laiye.EntUC.Core.dll                # 47 KB  - 用户中心核心库
│   └── Laiye.EntUC.Service.Grpc.dll        # 1.6 MB - 用户中心 gRPC 客户端
└── NuGet.Config                            # 新建：NuGet 配置文件
```

### 2. 修改的文件

**[Laiye.Customer.WebApi/Laiye.Customer.WebApi.csproj](Laiye.Customer.WebApi/Laiye.Customer.WebApi.csproj)**

将第 22-24 行的包引用：
```xml
<PackageReference Include="Laiye.EntCmd.Service.Grpc" Version="0.1.100001-feature-zhaoshangju" />
<PackageReference Include="Laiye.EntUC.Core" Version="0.1.200062" />
<PackageReference Include="Laiye.EntUC.Service.Grpc" Version="0.1.100003-feature-zsj" />
```

改为本地程序集引用：
```xml
<Reference Include="Laiye.EntCmd.Service.Grpc">
  <HintPath>..\local-packages\Laiye.EntCmd.Service.Grpc.dll</HintPath>
  <Private>True</Private>
</Reference>
<Reference Include="Laiye.EntUC.Core">
  <HintPath>..\local-packages\Laiye.EntUC.Core.dll</HintPath>
  <Private>True</Private>
</Reference>
<Reference Include="Laiye.EntUC.Service.Grpc">
  <HintPath>..\local-packages\Laiye.EntUC.Service.Grpc.dll</HintPath>
  <Private>True</Private>
</Reference>
```

## 使用说明

### 恢复依赖

在项目根目录执行：

```bash
dotnet restore
```

### 编译项目

```bash
dotnet build
```

### 运行项目

```bash
cd Laiye.Customer.WebApi
dotnet run
```

或者：

```bash
dotnet run --project Laiye.Customer.WebApi
```

## 注意事项

1. **不要删除 `local-packages` 文件夹**：包含项目运行所需的私有 DLL 文件

2. **不要提交到 Git**：这些私有 DLL 是公司内部资源，应该添加到 `.gitignore`：
   ```
   local-packages/
   ```

3. **依赖的 DLL 来源**：
   - 镜像：`registry.cn-beijing.aliyuncs.com/laiye-rpa/laiye-customer-webapi:v5.6.7-huaxiajijin-202401301720`
   - 提取位置：`backend-layer/app/`
   - 提取时间：2026-02-02

4. **如果需要更新私有包**：
   - 重新导出 Docker 镜像
   - 从新的镜像层中提取更新的 DLL 文件
   - 替换 `local-packages/` 中的对应文件

## 配置文件说明

### NuGet.Config

配置了本地包源，优先从 `local-packages` 文件夹查找包：

```xml
<packageSources>
  <add key="local" value="local-packages" />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```

## 故障排除

### 问题：找不到 Laiye.*.dll

**解决方案**：确认 `local-packages` 文件夹存在且包含三个 DLL 文件。

### 问题：编译失败，提示依赖错误

**解决方案**：可能是需要其他运行时依赖。检查 Docker 镜像中的完整依赖列表：

```bash
ls backend-layer/app/*.dll
```

### 问题：gRPC 连接失败

**解决方案**：检查 [appsettings.json](Laiye.Customer.WebApi/appsettings.json) 中的服务地址配置是否正确。

## 相关文件

- [NuGet.Config](NuGet.Config) - NuGet 配置文件
- [Laiye.Customer.WebApi.csproj](Laiye.Customer.WebApi/Laiye.Customer.WebApi.csproj) - 项目文件
- [appsettings.json](Laiye.Customer.WebApi/appsettings.json) - 应用配置
- [Program.cs](Laiye.Customer.WebApi/Program.cs) - 启动配置

## 版本信息

- 配置创建日期：2026-02-02
- DLL 源版本：v5.6.7-huaxiajijin-202401301720
- .NET 版本：.NET 6.0
