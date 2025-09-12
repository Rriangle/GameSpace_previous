# Database Documentation - GameSpace Platform

## Overview

This document provides comprehensive information about the database schema, tables, relationships, constraints, and seed data coverage for the GameSpace platform.

## Database Information

- **Database Type**: Microsoft SQL Server 2019/2022
- **Schema Source**: database.sql (single source of truth)
- **No EF Migrations**: Schema defined only in database.sql
- **Seed Data Rule**: Exactly 200 rows per table (idempotent)

## Core Tables

### Users Table
**Purpose**: User account information and authentication

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserId | int | PK, IDENTITY | Unique user identifier |
| Account | nvarchar(50) | NOT NULL, UNIQUE | User account name |
| Password | nvarchar(255) | NOT NULL | Hashed password |
| Nickname | nvarchar(100) | NOT NULL | Display name |
| Email | nvarchar(255) | NOT NULL, UNIQUE | Email address |
| Phone | nvarchar(20) | NULL | Phone number |
| Avatar | nvarchar(500) | NULL | Avatar image URL |
| Status | nvarchar(20) | NOT NULL, DEFAULT 'Active' | Account status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 users with realistic data

### User_Wallet Table
**Purpose**: User points and virtual currency management

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| WalletId | int | PK, IDENTITY | Unique wallet identifier |
| UserId | int | NOT NULL, FK to Users.UserId | User reference |
| Points | decimal(10,2) | NOT NULL, DEFAULT 0 | Available points |
| TotalEarned | decimal(10,2) | NOT NULL, DEFAULT 0 | Total points earned |
| TotalSpent | decimal(10,2) | NOT NULL, DEFAULT 0 | Total points spent |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 wallet records (one per user)

### CouponType Table
**Purpose**: Coupon type definitions

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| CouponTypeId | int | PK, IDENTITY | Unique coupon type identifier |
| TypeName | nvarchar(100) | NOT NULL | Coupon type name |
| Description | nvarchar(500) | NULL | Type description |
| DiscountType | nvarchar(20) | NOT NULL | Percentage or Fixed |
| DiscountValue | decimal(10,2) | NOT NULL | Discount amount |
| MinOrderAmount | decimal(10,2) | NULL | Minimum order requirement |
| MaxUses | int | NULL | Maximum usage limit |
| IsActive | bit | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 coupon types with various discount configurations

### Coupon Table
**Purpose**: Individual coupon instances

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| CouponId | int | PK, IDENTITY | Unique coupon identifier |
| CouponTypeId | int | NOT NULL, FK to CouponType.CouponTypeId | Coupon type reference |
| CouponCode | nvarchar(50) | NOT NULL, UNIQUE | Unique coupon code |
| UserId | int | NULL, FK to Users.UserId | Assigned user |
| IsUsed | bit | NOT NULL, DEFAULT 0 | Usage status |
| UsedAt | datetime2 | NULL | Usage timestamp |
| ExpiresAt | datetime2 | NOT NULL | Expiration date |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 coupons with realistic codes and expiration dates

### EVoucherType Table
**Purpose**: E-voucher type definitions

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| EVoucherTypeId | int | PK, IDENTITY | Unique e-voucher type identifier |
| TypeName | nvarchar(100) | NOT NULL | E-voucher type name |
| Description | nvarchar(500) | NULL | Type description |
| PointsCost | decimal(10,2) | NOT NULL | Points required to redeem |
| Value | decimal(10,2) | NOT NULL | E-voucher value |
| IsActive | bit | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 e-voucher types with various point costs and values

### EVoucher Table
**Purpose**: Individual e-voucher instances

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| EVoucherId | int | PK, IDENTITY | Unique e-voucher identifier |
| EVoucherTypeId | int | NOT NULL, FK to EVoucherType.EVoucherTypeId | E-voucher type reference |
| UserId | int | NOT NULL, FK to Users.UserId | Owner user |
| IsRedeemed | bit | NOT NULL, DEFAULT 0 | Redemption status |
| RedeemedAt | datetime2 | NULL | Redemption timestamp |
| ExpiresAt | datetime2 | NOT NULL | Expiration date |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 e-vouchers assigned to users

### EVoucherToken Table
**Purpose**: E-voucher redemption tokens

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| TokenId | int | PK, IDENTITY | Unique token identifier |
| EVoucherId | int | NOT NULL, FK to EVoucher.EVoucherId | E-voucher reference |
| TokenCode | nvarchar(100) | NOT NULL, UNIQUE | Unique token code |
| IsUsed | bit | NOT NULL, DEFAULT 0 | Usage status |
| UsedAt | datetime2 | NULL | Usage timestamp |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 tokens for e-voucher redemption

### EVoucherRedeemLog Table
**Purpose**: E-voucher redemption audit log

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| LogId | int | PK, IDENTITY | Unique log identifier |
| EVoucherId | int | NOT NULL, FK to EVoucher.EVoucherId | E-voucher reference |
| UserId | int | NOT NULL, FK to Users.UserId | User who redeemed |
| TokenId | int | NOT NULL, FK to EVoucherToken.TokenId | Token used |
| RedeemedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Redemption timestamp |
| PointsSpent | decimal(10,2) | NOT NULL | Points spent for redemption |

**Seed Data**: 200 redemption log entries

### WalletHistory Table
**Purpose**: Wallet transaction history

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| HistoryId | int | PK, IDENTITY | Unique history identifier |
| UserId | int | NOT NULL, FK to Users.UserId | User reference |
| TransactionType | nvarchar(50) | NOT NULL | Transaction type |
| Amount | decimal(10,2) | NOT NULL | Transaction amount |
| Balance | decimal(10,2) | NOT NULL | Balance after transaction |
| Description | nvarchar(500) | NULL | Transaction description |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Transaction timestamp |

**Seed Data**: 200 wallet history entries

### UserSignInStats Table
**Purpose**: Daily sign-in statistics

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| StatsId | int | PK, IDENTITY | Unique stats identifier |
| UserId | int | NOT NULL, FK to Users.UserId | User reference |
| SignInDate | date | NOT NULL | Sign-in date |
| PointsEarned | decimal(10,2) | NOT NULL, DEFAULT 0 | Points earned |
| StreakCount | int | NOT NULL, DEFAULT 1 | Current streak |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 sign-in statistics records

### Pets Table
**Purpose**: Virtual pet information

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| PetId | int | PK, IDENTITY | Unique pet identifier |
| UserId | int | NOT NULL, FK to Users.UserId | Owner user |
| PetName | nvarchar(100) | NOT NULL | Pet name |
| PetType | nvarchar(50) | NOT NULL, DEFAULT 'Slime' | Pet type |
| Level | int | NOT NULL, DEFAULT 1 | Pet level |
| Experience | decimal(10,2) | NOT NULL, DEFAULT 0 | Experience points |
| Hunger | int | NOT NULL, DEFAULT 50 | Hunger level (0-100) |
| Mood | int | NOT NULL, DEFAULT 50 | Mood level (0-100) |
| Energy | int | NOT NULL, DEFAULT 50 | Energy level (0-100) |
| Cleanliness | int | NOT NULL, DEFAULT 50 | Cleanliness level (0-100) |
| Health | int | NOT NULL, DEFAULT 50 | Health level (0-100) |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 pets with various attributes and levels

### Forums Table
**Purpose**: Forum categories

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ForumId | int | PK, IDENTITY | Unique forum identifier |
| GameId | int | NULL, FK to Games.GameId | Associated game |
| ForumName | nvarchar(100) | NOT NULL | Forum name |
| Description | nvarchar(500) | NULL | Forum description |
| ThreadCount | int | NOT NULL, DEFAULT 0 | Number of threads |
| PostCount | int | NOT NULL, DEFAULT 0 | Number of posts |
| LastActivity | datetime2 | NULL | Last activity timestamp |
| IsActive | bit | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 forum categories

### Threads Table
**Purpose**: Forum threads

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ThreadId | int | PK, IDENTITY | Unique thread identifier |
| ForumId | int | NOT NULL, FK to Forums.ForumId | Forum reference |
| UserId | int | NOT NULL, FK to Users.UserId | Thread author |
| Title | nvarchar(200) | NOT NULL | Thread title |
| Content | nvarchar(MAX) | NOT NULL | Thread content |
| ViewCount | int | NOT NULL, DEFAULT 0 | View count |
| ReplyCount | int | NOT NULL, DEFAULT 0 | Reply count |
| IsPinned | bit | NOT NULL, DEFAULT 0 | Pinned status |
| IsLocked | bit | NOT NULL, DEFAULT 0 | Locked status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 forum threads

### Posts Table
**Purpose**: Forum posts

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| PostId | int | PK, IDENTITY | Unique post identifier |
| ThreadId | int | NOT NULL, FK to Threads.ThreadId | Thread reference |
| UserId | int | NOT NULL, FK to Users.UserId | Post author |
| Content | nvarchar(MAX) | NOT NULL | Post content |
| IsEdited | bit | NOT NULL, DEFAULT 0 | Edited status |
| EditedAt | datetime2 | NULL | Edit timestamp |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 forum posts

### ProductInfo Table
**Purpose**: Store products

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ProductId | int | PK, IDENTITY | Unique product identifier |
| ProductName | nvarchar(200) | NOT NULL | Product name |
| Description | nvarchar(1000) | NULL | Product description |
| Price | decimal(10,2) | NOT NULL | Product price |
| Stock | int | NOT NULL, DEFAULT 0 | Stock quantity |
| Category | nvarchar(100) | NOT NULL | Product category |
| ImageUrl | nvarchar(500) | NULL | Product image URL |
| IsActive | bit | NOT NULL, DEFAULT 1 | Active status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 store products

### Orders Table
**Purpose**: Order information

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| OrderId | int | PK, IDENTITY | Unique order identifier |
| UserId | int | NOT NULL, FK to Users.UserId | Ordering user |
| OrderNumber | nvarchar(50) | NOT NULL, UNIQUE | Order number |
| TotalAmount | decimal(10,2) | NOT NULL | Total order amount |
| Status | nvarchar(50) | NOT NULL, DEFAULT 'Pending' | Order status |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |
| UpdatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Last update timestamp |

**Seed Data**: 200 orders

### Notifications Table
**Purpose**: System notifications

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| NotificationId | int | PK, IDENTITY | Unique notification identifier |
| NotificationTitle | nvarchar(200) | NOT NULL | Notification title |
| NotificationMessage | nvarchar(500) | NOT NULL | Notification message |
| SourceId | int | NULL, FK to NotificationSource.SourceId | Source reference |
| ActionId | int | NULL, FK to NotificationAction.ActionId | Action reference |
| SenderId | int | NULL, FK to Users.UserId | Sender user |
| SenderManagerId | int | NULL, FK to ManagerData.ManagerId | Sender manager |
| GroupId | int | NULL, FK to Groups.GroupId | Target group |
| CreatedAt | datetime2 | NOT NULL, DEFAULT GETUTCDATE() | Creation timestamp |

**Seed Data**: 200 system notifications

## Relationships

### Primary Relationships
- Users → User_Wallet (1:1)
- Users → Pets (1:Many)
- Users → UserSignInStats (1:Many)
- Users → Threads (1:Many)
- Users → Posts (1:Many)
- Users → Orders (1:Many)
- Users → EVouchers (1:Many)

### Coupon Relationships
- CouponType → Coupons (1:Many)
- Users → Coupons (1:Many)

### E-voucher Relationships
- EVoucherType → EVouchers (1:Many)
- Users → EVouchers (1:Many)
- EVouchers → EVoucherTokens (1:Many)
- EVouchers → EVoucherRedeemLog (1:Many)

### Forum Relationships
- Forums → Threads (1:Many)
- Threads → Posts (1:Many)
- Users → Threads (1:Many)
- Users → Posts (1:Many)

### Wallet Relationships
- Users → WalletHistory (1:Many)
- WalletHistory → Users (Many:1)

## Constraints

### Primary Keys
- All tables have integer primary keys with IDENTITY
- Primary keys are clustered indexes

### Foreign Keys
- All foreign key relationships are enforced
- Cascade delete rules applied where appropriate
- Referential integrity maintained

### Unique Constraints
- User Account names are unique
- User Email addresses are unique
- Coupon codes are unique
- E-voucher token codes are unique
- Order numbers are unique

### Check Constraints
- Status fields have predefined values
- Numeric fields have appropriate ranges
- Date fields have logical constraints

## Indexes

### Primary Indexes
- All primary keys are automatically indexed
- Clustered indexes on primary keys

### Foreign Key Indexes
- Indexes on all foreign key columns
- Non-clustered indexes for performance

### Search Indexes
- Full-text search indexes on content fields
- Composite indexes for common queries

## Seed Data Coverage

### Rule: Exactly 200 Rows Per Table
- All tables contain exactly 200 rows
- Data is idempotent (can be re-run safely)
- Batch size ≤ 1000 rows per operation
- Natural key hashing for reproducibility

### Data Quality
- Realistic, human-like data
- English text only
- Plausible URLs and images
- Zipf/log-normal distribution for amounts
- Small failure rate (0.5-2%) for realism

### Data Relationships
- All foreign key relationships maintained
- Referential integrity preserved
- Realistic data correlations
- Temporal consistency maintained

## Performance Considerations

### Query Optimization
- Read queries use AsNoTracking()
- Project to read models for aggregates
- Avoid returning entities directly
- Use appropriate indexes

### Connection Management
- Connection pooling enabled
- Timeout configurations set
- Retry policies implemented
- Resource cleanup ensured

### Caching Strategy
- Short-TTL cache for hot paths
- Cache invalidation on updates
- Memory-efficient caching
- Distributed cache support

## Security Considerations

### Data Protection
- Sensitive data encrypted
- Password hashing with bcrypt
- SQL injection prevention
- XSS protection

### Access Control
- Role-based permissions
- Resource-level authorization
- Audit logging enabled
- Data access monitoring

## Backup and Recovery

### Backup Strategy
- Full database backups daily
- Transaction log backups hourly
- Point-in-time recovery support
- Cross-region backup replication

### Recovery Procedures
- Automated recovery testing
- RTO/RPO targets defined
- Disaster recovery plans
- Data retention policies

## Monitoring and Maintenance

### Performance Monitoring
- Query performance tracking
- Resource utilization monitoring
- Index usage analysis
- Deadlock detection

### Maintenance Tasks
- Regular index maintenance
- Statistics updates
- Data archiving
- Cleanup procedures