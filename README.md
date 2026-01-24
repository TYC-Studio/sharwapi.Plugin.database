# Database Plugin

一个为(SharwAPI)[https://github.com/sharwapi]设计的数据库服务插件，提供灵活、可配置的数据库访问能力。

### 安装与配置

1. 把插件文件（一般为`sharwapi.Plugin.database.dll`）放入SharwApi框架运行目录的`/Plugins`文件夹下即可。
2. 在`appsettings.json`中添加`DatabasePlugin`字典：
- `ExposePluginWebApi` : 布尔值，决定插件是否暴露插件的网络Api，WIP（功能尚在开发）

## 使用示例

•	可以直接从 DI 获取`IDatabaseService`（设置`IDatabaseContextAccessor`），或者使用 `IDatabaseServiceFactory` 临时创建服务实例。

•	在每个作用域/请求中必须在**使用DatabaseService前**设置`DatabaseContext`（通过`IDatabaseContextAccessor`或`IDatabaseServiceFactory`）

### 代码用例（片段）
*包含两个示例*
```csharp
var builder = WebApplication.CreateBuilder(args);
// 注册 IDatabaseContextAccessor 实现
builder.Services.AddScoped<IDatabaseContextAccessor, ScopedDatabaseContextAccessor>();
var app = builder.Build();

//示例 1：使用Accessor配置上下文
app.MapGet("/users", async (IDatabaseContextAccessor accessor, IServiceProvider sp) =>
{
    // 设置当前作用域的连接字符串（也可以设置完整 DatabaseContext）
    var conn = app.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("No connection string configured");
    accessor.SetContext(new DatabaseContext { ConnectionString = conn });

    // 在设置上下文后解析 IDatabaseService（插件内部会从 accessor 读取上下文）
    var db = sp.GetRequiredService<IDatabaseService>();
    var rows = await db.QueryAsync<dynamic>("SELECT TOP 10 * FROM Users");
    return Results.Ok(rows);
});

// 示例 2：直接使用工厂临时创建服务实例（不依赖作用域 accessor）
app.MapGet("/test-connection", async (IDatabaseServiceFactory factory) =>
{
    var conn = app.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("No connection string configured");
    using var svc = factory.CreateService(conn) as IDisposable; // DatabaseService 实现了 IDisposable
    var db = factory.CreateService(conn);
    var ok = await db.TestConnectionAsync();
    return Results.Ok(new { ok });
});

//不要忘了
app.Run();
```

## 功能 & 核心接口

### `IDatabaseService`
数据库操作主接口，提供：
- `QueryAsync<T>` - 执行查询
- `ExecuteAsync` - 执行非查询命令
- `GetContext()` - 获取当前数据库上下文

### `IDatabaseServiceFactory`
数据库服务工厂，用于创建特定连接的数据库实例：
- `CreateService(string connectionString)` - 通过连接字符串创建
- `CreateService(DatabaseContext context)` - 通过完整上下文创建

### `DatabaseContext`
数据库上下文对象，包含：
- `ConnectionString` - 数据库连接字符串
- `CommandTimeout` - 命令超时时间
- `DatabaseType` - 数据库类型（SqlServer/PostgreSQL/MySQL）

## API端点

...WIP...（功能尚在开发）


## 依赖要求

- .NET 6.0 或更高版本
- Dapper（用于数据库操作）
- 主框架需支持插件机制和依赖注入
