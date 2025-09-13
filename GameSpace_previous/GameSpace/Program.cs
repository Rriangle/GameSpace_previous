using GameSpace.Infrastructure;
using GameSpace.Middleware;
using GameSpace.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Microsoft;
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

// 添加OAuth服務
builder.Services.AddScoped<OAuthService>();

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

// 添加認證服務
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    options.CallbackPath = "/OAuth/GoogleCallback";
})
.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
    options.CallbackPath = "/OAuth/FacebookCallback";
})
.AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? "";
    options.CallbackPath = "/OAuth/MicrosoftCallback";
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

// 添加認證和授權中介軟體
app.UseAuthentication();
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