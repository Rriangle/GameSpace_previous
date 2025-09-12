# GameSpace Operations Manual

## System Overview

### Technical Architecture
- **Backend**: ASP.NET Core 8.0 + Entity Framework Core
- **Database**: SQL Server 2022
- **Cache**: Memory Cache + Redis (optional)
- **Monitoring**: Serilog + Custom Performance Monitoring
- **Deployment**: IIS / Kestrel

### System Components
- **API Layer**: RESTful API + Swagger Documentation
- **Business Layer**: Wallet, Forum, Leaderboard, Pet, Store Systems
- **Data Layer**: Read/Write Repositories + Transaction Management
- **Monitoring Layer**: Health Checks + Performance Monitoring + Log Aggregation

## Deployment Guide

### 1. Environment Preparation

#### System Requirements
- Windows Server 2019+ or Linux (Ubuntu 20.04+)
- .NET 8.0 Runtime
- SQL Server 2019+ (recommended 2022)
- IIS 10+ (Windows) or Nginx (Linux)
- Minimum 4GB RAM, recommended 8GB+
- Minimum 50GB disk space

#### Network Requirements
- Port 80 (HTTP)
- Port 443 (HTTPS)
- Port 1433 (SQL Server)
- Port 6379 (Redis, if used)

### 2. Database Setup

#### SQL Server Configuration
```sql
-- Create database
CREATE DATABASE GameSpace;
GO

-- Create login
CREATE LOGIN gamespace_user WITH PASSWORD = 'SecurePassword123!';
GO

-- Create user and grant permissions
USE GameSpace;
CREATE USER gamespace_user FOR LOGIN gamespace_user;
ALTER ROLE db_owner ADD MEMBER gamespace_user;
GO
```

#### Database Initialization
```bash
# Run database schema and seed data
sqlcmd -S localhost -d GameSpace -i database.sql
```

### 3. Application Deployment

#### Windows Deployment (IIS)
1. Install .NET 8.0 Hosting Bundle
2. Create application pool
3. Configure website in IIS
4. Set connection string in web.config
5. Deploy application files

#### Linux Deployment (Kestrel)
1. Install .NET 8.0 Runtime
2. Create systemd service
3. Configure nginx reverse proxy
4. Set environment variables
5. Deploy application files

### 4. Configuration

#### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection="Server=localhost;Database=GameSpace;User Id=gamespace_user;Password=SecurePassword123!;"

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Logging
Serilog__MinimumLevel__Default=Information
Serilog__MinimumLevel__Override__Microsoft=Warning
```

#### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameSpace;User Id=gamespace_user;Password=SecurePassword123!;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/gamespace/gamespace-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

## Monitoring and Health Checks

### 1. Health Check Endpoints

#### Available Endpoints
- `/health` - Basic health check
- `/healthz` - Simple health check
- `/healthz/db` - Database connectivity check

#### Health Check Response
```json
{
  "status": "Healthy",
  "service": "GameSpace",
  "timestamp": "2025-01-09T18:30:00Z",
  "checks": {
    "database": "Healthy",
    "memory": "Healthy",
    "disk": "Healthy"
  }
}
```

### 2. Performance Monitoring

#### Key Metrics to Monitor
- Response time (P50, P95, P99)
- Throughput (requests per second)
- Error rate
- CPU usage
- Memory usage
- Database connection pool
- Cache hit rate

#### Monitoring Tools
- Application Insights (Azure)
- Prometheus + Grafana
- Custom performance counters
- Log aggregation (ELK Stack)

### 3. Logging

#### Log Levels
- **Trace**: Detailed debugging information
- **Debug**: Debugging information
- **Information**: General information
- **Warning**: Warning messages
- **Error**: Error messages
- **Critical**: Critical errors

#### Log Configuration
```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gamespace-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

## Maintenance Procedures

### 1. Regular Maintenance Tasks

#### Daily Tasks
- [ ] Check system health
- [ ] Review error logs
- [ ] Monitor performance metrics
- [ ] Verify backup completion
- [ ] Check disk space

#### Weekly Tasks
- [ ] Review security logs
- [ ] Update security patches
- [ ] Clean up old logs
- [ ] Review performance trends
- [ ] Test backup restoration

#### Monthly Tasks
- [ ] Review capacity planning
- [ ] Update documentation
- [ ] Review security policies
- [ ] Performance optimization
- [ ] Disaster recovery testing

### 2. Database Maintenance

#### Backup Strategy
```sql
-- Full backup (daily)
BACKUP DATABASE GameSpace TO DISK = 'C:\Backups\GameSpace_Full.bak'
WITH FORMAT, INIT, NAME = 'GameSpace Full Backup';

-- Transaction log backup (every 15 minutes)
BACKUP LOG GameSpace TO DISK = 'C:\Backups\GameSpace_Log.bak'
WITH FORMAT, INIT, NAME = 'GameSpace Log Backup';
```

#### Index Maintenance
```sql
-- Rebuild indexes
ALTER INDEX ALL ON User_Wallet REBUILD;
ALTER INDEX ALL ON Pet REBUILD;
ALTER INDEX ALL ON Thread REBUILD;
```

#### Statistics Update
```sql
-- Update statistics
UPDATE STATISTICS User_Wallet;
UPDATE STATISTICS Pet;
UPDATE STATISTICS Thread;
```

### 3. Application Maintenance

#### Log Rotation
```bash
# Rotate logs daily
logrotate /etc/logrotate.d/gamespace
```

#### Cache Management
```csharp
// Clear cache
await _cacheService.ClearAsync();

// Restart application pool
iisreset /restart
```

## Troubleshooting Guide

### 1. Common Issues

#### High CPU Usage
**Symptoms**: Slow response times, high CPU utilization
**Causes**: Inefficient queries, memory leaks, infinite loops
**Solutions**:
- Check database queries
- Review application logs
- Use profiling tools
- Optimize code

#### Memory Leaks
**Symptoms**: Increasing memory usage, eventual out of memory
**Causes**: Unclosed connections, event handlers, large objects
**Solutions**:
- Use using statements
- Dispose resources properly
- Monitor memory usage
- Use memory profiling tools

#### Database Connection Issues
**Symptoms**: Connection timeouts, connection pool exhaustion
**Causes**: Long-running queries, connection leaks, insufficient pool size
**Solutions**:
- Optimize queries
- Increase connection pool size
- Check connection management
- Monitor connection usage

#### Slow Response Times
**Symptoms**: High response times, user complaints
**Causes**: Database performance, network issues, inefficient code
**Solutions**:
- Check database performance
- Review query execution plans
- Optimize code
- Check network connectivity

### 2. Diagnostic Tools

#### Performance Counters
- CPU usage
- Memory usage
- Disk I/O
- Network I/O
- Database connections
- Request queue length

#### Log Analysis
```bash
# Search for errors
grep -i "error" /var/log/gamespace/gamespace-*.txt

# Search for slow queries
grep -i "slow" /var/log/gamespace/gamespace-*.txt

# Monitor real-time logs
tail -f /var/log/gamespace/gamespace-*.txt
```

#### Database Analysis
```sql
-- Check active connections
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('GameSpace');

-- Check long-running queries
SELECT * FROM sys.dm_exec_requests WHERE database_id = DB_ID('GameSpace');

-- Check index usage
SELECT * FROM sys.dm_db_index_usage_stats WHERE database_id = DB_ID('GameSpace');
```

### 3. Emergency Procedures

#### System Down
1. Check system status
2. Review recent changes
3. Check logs for errors
4. Restart services
5. Verify functionality
6. Notify stakeholders

#### Data Corruption
1. Stop application
2. Restore from backup
3. Verify data integrity
4. Restart application
5. Monitor for issues
6. Document incident

#### Security Incident
1. Isolate affected systems
2. Preserve evidence
3. Assess impact
4. Implement fixes
5. Monitor for recurrence
6. Report to authorities

## Security Operations

### 1. Security Monitoring

#### Security Logs
- Authentication failures
- Authorization errors
- SQL injection attempts
- XSS attempts
- Brute force attacks

#### Security Tools
- Web Application Firewall (WAF)
- Intrusion Detection System (IDS)
- Security Information and Event Management (SIEM)
- Vulnerability scanners

### 2. Access Control

#### User Management
- Regular access reviews
- Principle of least privilege
- Multi-factor authentication
- Account lockout policies
- Password policies

#### System Access
- Secure remote access
- VPN requirements
- Network segmentation
- Firewall rules
- Regular access audits

### 3. Data Protection

#### Encryption
- Data at rest encryption
- Data in transit encryption
- Key management
- Certificate management
- Regular key rotation

#### Backup Security
- Encrypted backups
- Secure storage
- Access controls
- Regular testing
- Offsite storage

## Disaster Recovery

### 1. Backup Strategy

#### Database Backups
- Full backups (daily)
- Differential backups (every 6 hours)
- Transaction log backups (every 15 minutes)
- Offsite storage
- Regular restoration testing

#### Application Backups
- Source code repositories
- Configuration files
- Application binaries
- Documentation
- Regular testing

### 2. Recovery Procedures

#### Database Recovery
1. Stop application
2. Restore full backup
3. Apply differential backups
4. Apply transaction log backups
5. Verify data integrity
6. Start application

#### Application Recovery
1. Deploy from backup
2. Restore configuration
3. Verify functionality
4. Monitor performance
5. Notify stakeholders

### 3. Business Continuity

#### RTO/RPO Targets
- Recovery Time Objective (RTO): 4 hours
- Recovery Point Objective (RPO): 1 hour
- Maximum downtime: 4 hours
- Data loss tolerance: 1 hour

#### Communication Plan
- Incident notification
- Status updates
- Stakeholder communication
- Media relations
- Post-incident review

## Performance Optimization

### 1. Database Optimization

#### Query Optimization
- Index optimization
- Query plan analysis
- Statistics updates
- Partitioning
- Archiving old data

#### Connection Optimization
- Connection pooling
- Connection timeouts
- Retry policies
- Load balancing
- Read replicas

### 2. Application Optimization

#### Code Optimization
- Algorithm optimization
- Memory management
- Caching strategies
- Async/await patterns
- Resource disposal

#### Configuration Optimization
- Cache settings
- Thread pool settings
- Memory limits
- Timeout values
- Retry policies

### 3. Infrastructure Optimization

#### Server Optimization
- CPU optimization
- Memory optimization
- Disk optimization
- Network optimization
- Load balancing

#### Monitoring Optimization
- Metric collection
- Alert thresholds
- Dashboard design
- Report generation
- Trend analysis

## Conclusion

This operations manual provides comprehensive guidance for operating and maintaining the GameSpace platform. Regular review and updates are essential to ensure the manual remains current and effective.

For questions or updates to this manual, please contact the operations team.