using GameSpace.Infrastructure;
using GameSpace.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gamespace-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 使用基礎設施專案添加服務
builder.Services.AddInfrastructure(builder.Configuration);

// 添加控制器
builder.Services.AddControllersWithViews();

// 添加Session支持
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 添加 API 探索器
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置 HTTP 請求管道
// 添加全域異常處理中介軟體
app.UseMiddleware<GlobalExceptionMiddleware>();

// 添加 CorrelationId 中介軟體
app.UseMiddleware<CorrelationIdMiddleware>();

// 添加速率限制中介軟體
app.UseRateLimit(maxRequests: 100, window: TimeSpan.FromMinutes(1));

// 添加超時中介軟體
app.UseTimeout(TimeSpan.FromSeconds(30));

// 添加 RBAC 權限檢查中介軟體
app.UseRBAC();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// 添加Session中介軟體
app.UseSession();

app.UseAuthorization();

app.MapControllers();

// 添加健康檢查端點
app.MapGet("/health", () => new { Status = "Healthy", Service = "GameSpace", Timestamp = DateTime.UtcNow });

// 添加簡單的 /healthz 端點
app.MapGet("/healthz", () => "healthy");

// 添加種子數據端點
app.MapPost("/seed", async (ISeedDataRunner seedRunner) =>
{
    try
    {
        await seedRunner.SeedAsync();
        return Results.Ok(new { Message = "種子數據創建成功", Timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem($"種子數據創建失敗: {ex.Message}");
    }
});

app.Run();