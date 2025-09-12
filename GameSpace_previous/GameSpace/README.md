# GameSpace

A .NET 8.0-based gaming platform solution providing user management, pet system, mini-games, marketplace, and community features.

## Project Structure

- **GameSpace.Api** - Web API project
- **GameSpace.Core** - Core business logic
- **GameSpace.Infrastructure** - Infrastructure layer
- **GameSpace.Web** - MVC Web application

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code

### Local Development

1. Clone the project
```bash
git clone <repository-url>
cd GameSpace
```

2. Restore packages
```bash
dotnet restore
```

3. Build solution
```bash
dotnet build
```

4. Run API
```bash
cd GameSpace.Api
dotnet run
```

5. Run Web application
```bash
cd GameSpace.Web
dotnet run
```

### Health Checks

API health check endpoints:
- GET /api/health - Basic health check
- GET /api/health/detailed - Detailed health check

## Feature Modules

### User Management
- User registration and login
- User profile management
- Permission control

### Pet System
- Pet nurturing
- Attribute upgrades
- Interactive features

### Mini Games
- Multiple mini-games
- Scoring system
- Leaderboards

### Marketplace System
- Product management
- Order processing
- Coupon system

### Community Features
- Forum discussions
- Article sharing
- Comment system

## Technology Stack

- **Backend**: .NET 8.0, ASP.NET Core, Entity Framework Core
- **Frontend**: ASP.NET Core MVC, Bootstrap, jQuery
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

## Development Guidelines

### Code Style
- Use English comments
- Follow C# naming conventions
- Use async/await for asynchronous operations

### Testing
- Unit tests
- Integration tests
- End-to-end tests

## License

This project is licensed under the MIT License.