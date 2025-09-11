using GameSpace.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace GameSpace.Tests
{
    /// <summary>
    /// Stage 3 廣度切片測試 - 寫入操作、交易處理、冪等性、審計
    /// </summary>
    public class Stage3WriteOperationsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public Stage3WriteOperationsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// 測試簽到 POST 端點
        /// </summary>
        [Fact]
        public async Task SignIn_ShouldReturnOk()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                UserId = 1,
                IdempotencyKey = $"test-signin-{Guid.NewGuid()}",
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/signin", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
            Assert.Contains("success", responseContent);
        }

        /// <summary>
        /// 測試簽到冪等性 - 相同的冪等性密鑰應該返回相同結果
        /// </summary>
        [Fact]
        public async Task SignIn_WithSameIdempotencyKey_ShouldReturnSameResult()
        {
            // Arrange
            var idempotencyKey = $"test-idempotent-{Guid.NewGuid()}";
            var signInRequest = new SignInRequest
            {
                UserId = 2,
                IdempotencyKey = idempotencyKey,
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content1 = new StringContent(json, Encoding.UTF8, "application/json");
            var content2 = new StringContent(json, Encoding.UTF8, "application/json");

            // Act - 發送兩次相同的請求
            var response1 = await _client.PostAsync("/api/signin", content1);
            var response2 = await _client.PostAsync("/api/signin", content2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            // 第二次請求可能返回 Conflict (409) 表示已經簽到過，或者返回相同的 OK 結果
            Assert.True(response2.StatusCode == HttpStatusCode.OK || response2.StatusCode == HttpStatusCode.Conflict);

            var content1Text = await response1.Content.ReadAsStringAsync();
            var content2Text = await response2.Content.ReadAsStringAsync();
            
            // 兩次響應都應該包含相同的冪等性密鑰
            Assert.Contains(idempotencyKey, content1Text);
            Assert.Contains(idempotencyKey, content2Text);
        }

        /// <summary>
        /// 測試簽到請求驗證 - 無效的用戶 ID
        /// </summary>
        [Fact]
        public async Task SignIn_WithInvalidUserId_ShouldReturnBadRequest()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                UserId = 0, // 無效的用戶 ID
                IdempotencyKey = $"test-invalid-{Guid.NewGuid()}",
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/signin", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("無效", responseContent);
        }

        /// <summary>
        /// 測試簽到請求驗證 - 缺少冪等性密鑰
        /// </summary>
        [Fact]
        public async Task SignIn_WithoutIdempotencyKey_ShouldReturnBadRequest()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                UserId = 3,
                IdempotencyKey = "", // 空的冪等性密鑰
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/signin", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("冪等性", responseContent);
        }

        /// <summary>
        /// 測試簽到統計查詢端點
        /// </summary>
        [Fact]
        public async Task GetSignInStats_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/signin/stats/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains("userId", content);
        }

        /// <summary>
        /// 測試冪等性檢查端點
        /// </summary>
        [Fact]
        public async Task CheckIdempotency_ShouldReturnOk()
        {
            // Arrange
            var idempotencyKey = $"test-check-{Guid.NewGuid()}";

            // Act
            var response = await _client.GetAsync($"/api/signin/idempotency/{idempotencyKey}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains("exists", content);
        }

        /// <summary>
        /// 測試所有端點都包含 CorrelationId 標頭（繼承自 Stage 0）
        /// </summary>
        [Fact]
        public async Task AllStage3Endpoints_ShouldIncludeCorrelationIdHeader()
        {
            // Test GET endpoints
            var getEndpoints = new[]
            {
                "/api/signin/stats/1",
                "/api/signin/idempotency/test-correlation"
            };

            foreach (var endpoint in getEndpoints)
            {
                // Act
                var response = await _client.GetAsync(endpoint);

                // Assert
                Assert.True(response.Headers.Contains("X-Correlation-ID"), 
                    $"GET endpoint {endpoint} should include X-Correlation-ID header");
            }

            // Test POST endpoint
            var signInRequest = new SignInRequest
            {
                UserId = 1,
                IdempotencyKey = $"test-correlation-{Guid.NewGuid()}",
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/api/signin", content);
            Assert.True(postResponse.Headers.Contains("X-Correlation-ID"), 
                "POST /api/signin should include X-Correlation-ID header");
        }

        /// <summary>
        /// 測試無效用戶 ID 的簽到統計查詢
        /// </summary>
        [Fact]
        public async Task GetSignInStats_WithInvalidUserId_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/signin/stats/0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
