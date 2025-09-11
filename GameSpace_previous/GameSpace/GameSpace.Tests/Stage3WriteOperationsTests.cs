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
    /// Stage 3 �s�פ������� - �g�J�ާ@�B����B�z�B�����ʡB�f�p
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
        /// ����ñ�� POST ���I
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
        /// ����ñ�쾭���� - �ۦP�������ʱK�_���Ӫ�^�ۦP���G
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

            // Act - �o�e�⦸�ۦP���ШD
            var response1 = await _client.PostAsync("/api/signin", content1);
            var response2 = await _client.PostAsync("/api/signin", content2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            // �ĤG���ШD�i���^ Conflict (409) ��ܤw�gñ��L�A�Ϊ̪�^�ۦP�� OK ���G
            Assert.True(response2.StatusCode == HttpStatusCode.OK || response2.StatusCode == HttpStatusCode.Conflict);

            var content1Text = await response1.Content.ReadAsStringAsync();
            var content2Text = await response2.Content.ReadAsStringAsync();
            
            // �⦸�T�������ӥ]�t�ۦP�������ʱK�_
            Assert.Contains(idempotencyKey, content1Text);
            Assert.Contains(idempotencyKey, content2Text);
        }

        /// <summary>
        /// ����ñ��ШD���� - �L�Ī��Τ� ID
        /// </summary>
        [Fact]
        public async Task SignIn_WithInvalidUserId_ShouldReturnBadRequest()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                UserId = 0, // �L�Ī��Τ� ID
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
            Assert.Contains("�L��", responseContent);
        }

        /// <summary>
        /// ����ñ��ШD���� - �ʤ־����ʱK�_
        /// </summary>
        [Fact]
        public async Task SignIn_WithoutIdempotencyKey_ShouldReturnBadRequest()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                UserId = 3,
                IdempotencyKey = "", // �Ū������ʱK�_
                SignInType = "daily"
            };

            var json = JsonSerializer.Serialize(signInRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/signin", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("������", responseContent);
        }

        /// <summary>
        /// ����ñ��έp�d�ߺ��I
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
        /// ���վ������ˬd���I
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
        /// ���թҦ����I���]�t CorrelationId ���Y�]�~�Ӧ� Stage 0�^
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
        /// ���յL�ĥΤ� ID ��ñ��έp�d��
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
