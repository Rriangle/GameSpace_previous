using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GameSpace.Tests
{
    /// <summary>
    /// Stage 2 廣度切片測試 - 錢包、論壇、排行榜聚合查詢端點
    /// </summary>
    public class Stage2BreadthSliceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public Stage2BreadthSliceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// 測試錢包總覽端點
        /// </summary>
        [Fact]
        public async Task WalletOverview_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/wallet/overview/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains("userId", content);
        }

        /// <summary>
        /// 測試用戶積分查詢端點
        /// </summary>
        [Fact]
        public async Task WalletPoints_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/wallet/points/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        /// <summary>
        /// 測試論壇列表端點
        /// </summary>
        [Fact]
        public async Task ForumList_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/forum/list");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        /// <summary>
        /// 測試排行榜總覽端點
        /// </summary>
        [Fact]
        public async Task LeaderboardOverview_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/leaderboard/overview");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains("dailyLeaderboard", content);
        }

        /// <summary>
        /// 測試每日排行榜端點
        /// </summary>
        [Fact]
        public async Task DailyLeaderboard_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/leaderboard/period/daily");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        /// <summary>
        /// 測試 CorrelationId 標頭是否存在（繼承自 Stage 0）
        /// </summary>
        [Fact]
        public async Task AllEndpoints_ShouldIncludeCorrelationIdHeader()
        {
            // Test multiple endpoints
            var endpoints = new[]
            {
                "/api/wallet/overview/1",
                "/api/forum/list",
                "/api/leaderboard/overview"
            };

            foreach (var endpoint in endpoints)
            {
                // Act
                var response = await _client.GetAsync(endpoint);

                // Assert
                Assert.True(response.Headers.Contains("X-Correlation-ID"), 
                    $"Endpoint {endpoint} should include X-Correlation-ID header");
            }
        }
    }
}
