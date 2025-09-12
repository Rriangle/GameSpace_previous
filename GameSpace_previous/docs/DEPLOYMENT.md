# Deployment Guide - GameSpace Platform

## Overview

This document provides comprehensive deployment instructions for the GameSpace platform on GitHub Actions CI/CD and Google Cloud Platform (GCP).

## Prerequisites

- .NET 8.0 SDK
- SQL Server 2019/2022
- GitHub repository with Actions enabled
- GCP project with appropriate permissions
- Docker (for containerization)

## GitHub Actions CI/CD

### Workflow Triggers

- Push to main branch
- Release tags
- Pull request to main branch

### Quality Gates

- .NET 8 build verification
- All tests must pass (unit, integration, E2E)
- Code analyzers with 0 warnings
- NuGet package cache for performance
- Security scanning

### Artifacts

- Container image pushed to GCP Artifact Registry
- Build artifacts for manual deployment
- Test reports and coverage

### Security

- OIDC (Workload Identity Federation) to GCP
- Minimal-privilege Service Accounts
- Secrets managed via GitHub Secrets
- No secrets in source code

## Google Cloud Platform Deployment

### Option 1: Cloud Run (Recommended)

#### Container Configuration
- Port: 8080 (default)
- CPU: 1-2 vCPU
- Memory: 2-4 GB
- Max concurrency: 100
- Min instances: 0 (or 1 to reduce cold starts)

#### Database
- Cloud SQL for SQL Server
- Private IP preferred
- VPC connector for secure connection
- Connection pooling enabled

#### Environment Variables
- ASPNETCORE_ENVIRONMENT=Production
- Connection string via Secret Manager
- Feature flags configuration

#### Health Checks
- Liveness probe: /healthz
- Readiness probe: /healthz/db
- Serilog logs visible in Cloud Logging

### Option 2: Compute Engine (Alternative)

#### Runtime
- Kestrel web server
- Auto-start enabled
- Firewall: HTTP/HTTPS ports

#### Database
- Cloud SQL for SQL Server (Private IP)
- Connection via VPC

#### Monitoring
- Cloud Monitoring integration
- Cloud Logging integration
- Alert configuration

## Environment Variables & Secrets

| Variable | Description | Required | Source |
|----------|-------------|----------|---------|
| DefaultConnection | SQL Server connection string | Yes | Secret Manager |
| ASPNETCORE_ENVIRONMENT | Environment (Production/Staging) | Yes | Environment |
| GCP_PROJECT_ID | GCP Project ID | Yes | Environment |
| GCP_REGION | Deployment region | Yes | Environment |
| CLOUD_SQL_CONN_NAME | Cloud SQL connection name | Yes | Secret Manager |
| CONNECTION_STR | Alternative connection string | No | Secret Manager |
| Feature flags | Feature toggle configuration | No | Environment |

## Database Setup

### Initial Setup
1. Run database.sql in Cloud SQL
2. Verify all tables created
3. Run seed data generation
4. Verify 200 rows per table

### Connection Security
- Use Private IP for Cloud SQL
- VPC connector for Cloud Run
- Service account authentication
- SSL/TLS encryption

## Health Checks

### Endpoints
- `/healthz` - Basic health check
- `/healthz/db` - Database connectivity check
- `/healthz/detailed` - Detailed system status

### Monitoring
- Response time monitoring
- Error rate tracking
- Resource utilization
- Database performance

## Rollback Strategy

### Cloud Run
- Revision-based rollback
- Traffic splitting for gradual rollback
- Blue-green deployment support

### Compute Engine
- VM image snapshots
- Load balancer configuration
- Database backup restoration

## Release Management

### Versioning
- Semantic versioning (SemVer)
- Git tags for releases
- Changelog maintenance

### Deployment Process
1. Code review and approval
2. Automated testing
3. Build and containerization
4. Staging deployment
5. Production deployment
6. Health check verification

## Troubleshooting

### Common Issues
- Connection string configuration
- Database connectivity problems
- Memory/CPU resource limits
- Cold start performance

### Logs
- Application logs: Cloud Logging
- System logs: Compute Engine logs
- Database logs: Cloud SQL logs

### Monitoring
- Cloud Monitoring dashboards
- Alert policies
- Performance metrics

## Security Considerations

- All secrets in Secret Manager
- Network security groups
- SSL/TLS certificates
- Regular security updates
- Access control and IAM

## Performance Optimization

- Connection pooling
- Caching strategies
- CDN for static assets
- Database query optimization
- Resource scaling

## Backup and Recovery

- Database automated backups
- Application state backup
- Disaster recovery procedures
- Data retention policies