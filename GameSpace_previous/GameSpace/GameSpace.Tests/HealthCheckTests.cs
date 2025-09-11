using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using GameSpace.Api;

namespace GameSpace.Tests
{
    /// <summary>
    /// °·±dÀË¬d´ú¸Õ
    /// </summary>
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HealthCheckTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Healthz_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/healthz");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        }
    }
}
