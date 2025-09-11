# GameSpace 測試政策

## 測試策略概覽

### 測試金字塔
```
        /\
       /  \
      / E2E \     <- 少量端到端測試（關鍵流程）
     /______\
    /        \
   /Integration\ <- 適量整合測試（模組間互動）
  /____________\
 /              \
/    Unit Tests   \ <- 大量單元測試（業務邏輯）
/__________________\
```

### 測試覆蓋率目標
- **單元測試**: 80%+ 程式碼覆蓋率
- **整合測試**: 70%+ 關鍵路徑覆蓋率
- **端到端測試**: 100% 關鍵使用者流程覆蓋率

## 單元測試

### 測試範圍
- 業務邏輯服務
- 資料存取層方法
- 工具類和輔助方法
- 計算邏輯和演算法

### 測試原則
```csharp
// 1. 遵循 AAA 模式
[Fact]
public void CalculateUserLevel_WithValidExperience_ShouldReturnCorrectLevel()
{
    // Arrange - 準備測試資料
    var user = new User { Experience = 1500 };
    var levelCalculator = new LevelCalculator();
    
    // Act - 執行被測試的方法
    var result = levelCalculator.CalculateLevel(user.Experience);
    
    // Assert - 驗證結果
    Assert.Equal(15, result);
}

// 2. 使用描述性的測試名稱
[Fact]
public void WalletService_AddPoints_WhenUserExists_ShouldUpdatePointsAndLogTransaction()
{
    // 測試錢包服務新增點數功能
}

// 3. 測試邊界條件
[Theory]
[InlineData(0, 0)]
[InlineData(100, 1)]
[InlineData(1000, 10)]
[InlineData(9999, 99)]
public void CalculateLevel_WithVariousExperience_ShouldReturnCorrectLevel(int experience, int expectedLevel)
{
    var result = _levelCalculator.CalculateLevel(experience);
    Assert.Equal(expectedLevel, result);
}
```

### 測試資料管理
```csharp
// 1. 使用測試資料建構器
public class UserBuilder
{
    private User _user = new User();
    
    public UserBuilder WithId(int id)
    {
        _user.Id = id;
        return this;
    }
    
    public UserBuilder WithExperience(int experience)
    {
        _user.Experience = experience;
        return this;
    }
    
    public User Build() => _user;
}

// 2. 使用測試資料工廠
public static class TestDataFactory
{
    public static User CreateUser(int id = 1, string username = "testuser")
    {
        return new User
        {
            Id = id,
            Username = username,
            Email = $"{username}@test.com",
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

## 整合測試

### 測試範圍
- API 端點整合
- 資料庫操作整合
- 外部服務整合
- 中介軟體整合

### 測試設定
```csharp
public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly GameSpaceContext Context;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        Context = Scope.ServiceProvider.GetRequiredService<GameSpaceContext>();
    }

    protected async Task SeedTestDataAsync()
    {
        // 種子測試資料
        var users = new List<User>
        {
            TestDataFactory.CreateUser(1, "user1"),
            TestDataFactory.CreateUser(2, "user2")
        };
        
        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context?.Dispose();
        Scope?.Dispose();
        Client?.Dispose();
    }
}
```

### 資料庫整合測試
```csharp
[Fact]
public async Task WalletRepository_GetWalletOverview_ShouldReturnCorrectData()
{
    // Arrange
    await SeedTestDataAsync();
    var repository = new WalletReadOnlyRepository(Context);
    
    // Act
    var result = await repository.GetWalletOverviewAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.UserId);
    Assert.True(result.Points >= 0);
}

[Fact]
public async Task SignInWriteRepository_ProcessSignIn_ShouldUpdateDatabase()
{
    // Arrange
    await SeedTestDataAsync();
    var repository = new SignInWriteRepository(Context);
    var idempotencyKey = Guid.NewGuid().ToString();
    
    // Act
    var result = await repository.ProcessSignInAsync(1, idempotencyKey);
    
    // Assert
    Assert.True(result.Success);
    
    // 驗證資料庫更新
    var user = await Context.Users.FindAsync(1);
    Assert.True(user.LastSignInDate.HasValue);
    
    var wallet = await Context.User_Wallet.FirstOrDefaultAsync(w => w.UserId == 1);
    Assert.NotNull(wallet);
    Assert.True(wallet.Points > 0);
}
```

## 端到端測試

### 測試範圍
- 關鍵使用者流程
- 跨模組整合
- 外部依賴整合
- 效能關鍵路徑

### 測試設定
```csharp
public class E2ETestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    public E2ETestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> SignInUserAsync(string username, string password)
    {
        var loginData = new { Username = username, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
        return await Client.PostAsync("/api/auth/signin", content);
    }

    protected async Task<HttpResponseMessage> GetWalletOverviewAsync(int userId)
    {
        return await Client.GetAsync($"/api/wallet/overview?userId={userId}");
    }
}
```

### 關鍵流程測試
```csharp
[Fact]
public async Task CompleteUserJourney_FromSignInToGamePlay_ShouldWorkCorrectly()
{
    // 1. 使用者登入
    var signInResponse = await SignInUserAsync("testuser", "password123");
    Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);
    
    var signInResult = await signInResponse.Content.ReadFromJsonAsync<SignInResult>();
    Assert.NotNull(signInResult);
    Assert.True(signInResult.Success);
    
    // 2. 檢查錢包狀態
    var walletResponse = await GetWalletOverviewAsync(signInResult.UserId);
    Assert.Equal(HttpStatusCode.OK, walletResponse.StatusCode);
    
    var wallet = await walletResponse.Content.ReadFromJsonAsync<WalletOverviewReadModel>();
    Assert.NotNull(wallet);
    Assert.True(wallet.Points > 0);
    
    // 3. 瀏覽論壇
    var forumResponse = await Client.GetAsync("/api/forum/list?page=1&pageSize=10");
    Assert.Equal(HttpStatusCode.OK, forumResponse.StatusCode);
    
    // 4. 查看排行榜
    var leaderboardResponse = await Client.GetAsync("/api/leaderboard/daily");
    Assert.Equal(HttpStatusCode.OK, leaderboardResponse.StatusCode);
    
    // 5. 執行小遊戲
    var gameResponse = await Client.PostAsync("/api/minigame/start", null);
    Assert.Equal(HttpStatusCode.OK, gameResponse.StatusCode);
}
```

## 效能測試

### 基準測試
```csharp
[Fact]
public async Task ApiEndpoint_ShouldRespondWithinBaseline()
{
    var stopwatch = Stopwatch.StartNew();
    
    var response = await Client.GetAsync("/api/wallet/overview?userId=1");
    
    stopwatch.Stop();
    
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
        $"響應時間 {stopwatch.ElapsedMilliseconds}ms 超過基準線 1000ms");
}
```

### 負載測試
```csharp
[Fact]
public async Task ConcurrentRequests_ShouldMaintainPerformance()
{
    const int concurrentRequests = 20;
    var tasks = new List<Task<HttpResponseMessage>>();
    
    for (int i = 0; i < concurrentRequests; i++)
    {
        tasks.Add(Client.GetAsync("/healthz"));
    }
    
    var responses = await Task.WhenAll(tasks);
    
    Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
    
    var averageResponseTime = responses
        .Select(r => r.Headers.GetValues("X-Response-Time").FirstOrDefault()?.ParseInt() ?? 0)
        .Average();
    
    Assert.True(averageResponseTime < 500, $"平均響應時間 {averageResponseTime}ms 超過基準線");
}
```

## 測試資料管理

### 測試資料庫
```csharp
public class TestDatabaseFixture : IAsyncLifetime
{
    public GameSpaceContext Context { get; private set; }
    private string _connectionString;

    public async Task InitializeAsync()
    {
        // 建立測試資料庫
        _connectionString = "Server=(localdb)\\mssqllocaldb;Database=GameSpaceTest;Trusted_Connection=true;";
        
        var options = new DbContextOptionsBuilder<GameSpaceContext>()
            .UseSqlServer(_connectionString)
            .Options;
        
        Context = new GameSpaceContext(options);
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        Context?.Dispose();
    }
}
```

### 測試資料種子
```csharp
public static class TestDataSeeder
{
    public static async Task SeedUsersAsync(GameSpaceContext context)
    {
        var users = new List<User>
        {
            new User { Id = 1, Username = "testuser1", Email = "user1@test.com" },
            new User { Id = 2, Username = "testuser2", Email = "user2@test.com" }
        };
        
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    public static async Task SeedWalletsAsync(GameSpaceContext context)
    {
        var wallets = new List<User_Wallet>
        {
            new User_Wallet { UserId = 1, Points = 1000, Coupons = 5 },
            new User_Wallet { UserId = 2, Points = 2000, Coupons = 10 }
        };
        
        context.User_Wallet.AddRange(wallets);
        await context.SaveChangesAsync();
    }
}
```

## 測試自動化

### CI/CD 整合
```yaml
# .github/workflows/test.yml
name: Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Upload coverage
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage.cobertura.xml
```

### 測試執行策略
```bash
# 1. 快速測試（開發時）
dotnet test --filter "Category=Unit" --logger "console;verbosity=minimal"

# 2. 完整測試（CI/CD）
dotnet test --logger "console;verbosity=normal" --collect:"XPlat Code Coverage"

# 3. 效能測試（定期）
dotnet test --filter "Category=Performance" --logger "console;verbosity=detailed"

# 4. 整合測試（部署前）
dotnet test --filter "Category=Integration" --logger "console;verbosity=normal"
```

## 測試品質保證

### 程式碼覆蓋率
```xml
<!-- coverlet.runsettings -->
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Threshold>80</Threshold>
          <ThresholdType>line</ThresholdType>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### 測試分類
```csharp
// 使用測試分類標籤
[Fact]
[Trait("Category", "Unit")]
[Trait("Module", "Wallet")]
public void WalletService_AddPoints_ShouldUpdatePoints()
{
    // 單元測試
}

[Fact]
[Trait("Category", "Integration")]
[Trait("Module", "Wallet")]
public async Task WalletApi_GetOverview_ShouldReturnData()
{
    // 整合測試
}

[Fact]
[Trait("Category", "Performance")]
[Trait("Module", "System")]
public async Task System_UnderLoad_ShouldMaintainPerformance()
{
    // 效能測試
}
```

## 測試最佳實踐

### 1. 測試命名
- 使用描述性的測試方法名稱
- 遵循 `MethodName_Scenario_ExpectedResult` 格式
- 使用繁體中文註解說明測試目的

### 2. 測試組織
- 按功能模組組織測試
- 使用測試類別分組相關測試
- 使用測試資料夾結構

### 3. 測試維護
- 定期檢視和更新測試
- 移除過時或重複的測試
- 保持測試的獨立性和可重複性

### 4. 測試文件
- 為複雜的測試邏輯添加註解
- 記錄測試的假設和限制
- 維護測試執行指南

## 測試工具和框架

### 主要工具
- **xUnit**: 測試框架
- **Moq**: 模擬框架
- **FluentAssertions**: 斷言庫
- **AutoFixture**: 測試資料生成
- **Coverlet**: 程式碼覆蓋率

### 輔助工具
- **TestContainers**: 容器化測試環境
- **WireMock**: API 模擬
- **NBomber**: 負載測試
- **ReportGenerator**: 覆蓋率報告

---

**文件版本**: 1.0  
**建立日期**: 2025-01-09  
**最後更新**: 2025-01-09  
**負責人**: GameSpace 測試團隊