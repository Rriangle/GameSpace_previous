using GameSpace.Infrastructure;
using GameSpace.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gamespace-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services using Infrastructure project
builder.Services.AddInfrastructure(builder.Configuration);

// Add custom services
builder.Services.AddScoped<GameSpace.Services.Authentication.IAuthService, GameSpace.Services.Authentication.AuthService>();
builder.Services.AddScoped<GameSpace.Services.Wallet.IWalletService, GameSpace.Services.Wallet.WalletService>();
builder.Services.AddScoped<GameSpace.Services.SignIn.ISignInService, GameSpace.Services.SignIn.SignInService>();
builder.Services.AddScoped<GameSpace.Services.Pet.IPetService, GameSpace.Services.Pet.PetService>();
builder.Services.AddScoped<GameSpace.Services.MiniGame.IMiniGameService, GameSpace.Services.MiniGame.MiniGameService>();
builder.Services.AddScoped<GameSpace.Services.Forum.IForumService, GameSpace.Services.Forum.ForumService>();
builder.Services.AddScoped<GameSpace.Services.Social.ISocialService, GameSpace.Services.Social.SocialService>();
builder.Services.AddScoped<GameSpace.Services.Store.IStoreService, GameSpace.Services.Store.StoreService>();
builder.Services.AddScoped<GameSpace.Services.Admin.IAdminService, GameSpace.Services.Admin.AdminService>();

// Add controllers and views
builder.Services.AddControllersWithViews();

// Add session support for shopping cart
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
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

// Configure the HTTP request pipeline
// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Add CorrelationId middleware
app.UseMiddleware<CorrelationIdMiddleware>();

// Add rate limiting middleware
app.UseRateLimit(maxRequests: 100, window: TimeSpan.FromMinutes(1));

// Add timeout middleware
app.UseTimeout(TimeSpan.FromSeconds(30));

// Add RBAC permission check middleware
app.UseRBAC();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Add health check endpoints
app.MapGet("/health", () => new { Status = "Healthy", Service = "GameSpace", Timestamp = DateTime.UtcNow });

// Add simple /healthz endpoint
app.MapGet("/healthz", () => "healthy");

// Add database connectivity check endpoint
app.MapGet("/healthz/db", async (IServiceProvider services) =>
{
    try
    {
        // Check database connectivity
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameSpaceDbContext>();
        await context.Database.CanConnectAsync();
        return Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connectivity check failed: {ex.Message}");
    }
});

// Add seed data endpoint
app.MapPost("/seed", async (ISeedDataRunner seedRunner) =>
{
    try
    {
        await seedRunner.SeedAsync();
        return Results.Ok(new { Message = "Seed data creation successful", Timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Seed data creation failed: {ex.Message}");
    }
});

app.Run();