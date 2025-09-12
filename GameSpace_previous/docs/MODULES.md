# Modules Documentation - GameSpace Platform

## Overview

This document provides comprehensive information about all modules in the GameSpace platform, including features, routes/endpoints, DTO/OpenAPI references, and UI affiliations.

## Area Structure

The platform is organized into the following Areas:

- **MiniGame** - Public UI
- **Forum** - Public UI  
- **MemberManagement** - Admin UI
- **OnlineStore** - Public UI
- **social_hub** - Mixed UI (Admin + Public)

## MiniGame Area (Public UI)

### User_Wallet Module
**UI Affiliation**: Public  
**Description**: Points, coupons, and e-vouchers management

#### Features
- Points balance display
- Available coupons browsing
- E-voucher token management
- Wallet history tracking

#### Routes/Endpoints
- `GET /MiniGame/Wallet` - Wallet overview
- `GET /MiniGame/Wallet/History` - Transaction history
- `POST /MiniGame/Wallet/Redeem` - Coupon redemption

#### DTOs
- `WalletViewModel` - Wallet display data
- `CouponViewModel` - Coupon information
- `TransactionHistoryViewModel` - Transaction details

### UserSignInStats Module
**UI Affiliation**: Public  
**Description**: Daily sign-in statistics and rewards

#### Features
- Daily sign-in interface
- Sign-in streak tracking
- Reward distribution
- Statistics dashboard

#### Routes/Endpoints
- `GET /MiniGame/SignIn` - Sign-in page
- `POST /MiniGame/SignIn/CheckIn` - Perform sign-in
- `GET /MiniGame/SignIn/Stats` - User statistics

#### DTOs
- `SignInViewModel` - Sign-in form data
- `SignInStatsViewModel` - Statistics display

### Pet Module
**UI Affiliation**: Public  
**Description**: Virtual pet management and interaction

#### Features
- Pet profile display
- Attribute management (hunger, mood, energy, cleanliness, health)
- Pet leveling system
- Interactive animations

#### Routes/Endpoints
- `GET /MiniGame/Pet` - Pet profile
- `POST /MiniGame/Pet/Feed` - Feed pet
- `POST /MiniGame/Pet/Play` - Play with pet
- `POST /MiniGame/Pet/Clean` - Clean pet

#### DTOs
- `PetViewModel` - Pet display data
- `PetAttributeViewModel` - Attribute information

### MiniGame Module
**UI Affiliation**: Public  
**Description**: Mini-games and adventures

#### Features
- Game list interface
- Game play mechanics
- Score tracking
- Leaderboards

#### Routes/Endpoints
- `GET /MiniGame/Games` - Game list
- `GET /MiniGame/Games/{id}` - Game details
- `POST /MiniGame/Games/{id}/Play` - Start game
- `GET /MiniGame/Leaderboard` - Leaderboard

#### DTOs
- `GameViewModel` - Game information
- `GameResultViewModel` - Game results
- `LeaderboardViewModel` - Leaderboard data

## Forum Area (Public UI)

### Forum Discussion Module
**UI Affiliation**: Public  
**Description**: Community discussion forums

#### Features
- Forum category browsing
- Thread creation and management
- Post creation and editing
- User reputation system

#### Routes/Endpoints
- `GET /Forum` - Forum home
- `GET /Forum/Category/{id}` - Category threads
- `GET /Forum/Thread/{id}` - Thread details
- `POST /Forum/Thread/Create` - Create thread
- `POST /Forum/Post/Create` - Create post

#### DTOs
- `ForumCategoryViewModel` - Category information
- `ThreadViewModel` - Thread display data
- `PostViewModel` - Post information

### Article Sharing Module
**UI Affiliation**: Public  
**Description**: User-generated content sharing

#### Features
- Article creation and editing
- Article categorization
- Like and comment system
- Article search and filtering

#### Routes/Endpoints
- `GET /Forum/Articles` - Article list
- `GET /Forum/Articles/{id}` - Article details
- `POST /Forum/Articles/Create` - Create article
- `POST /Forum/Articles/{id}/Like` - Like article

#### DTOs
- `ArticleViewModel` - Article display data
- `ArticleCreateViewModel` - Article creation form

## MemberManagement Area (Admin UI)

### User Management Module
**UI Affiliation**: Admin  
**Description**: User account management and administration

#### Features
- User list and search
- User profile management
- Account status control
- User activity monitoring

#### Routes/Endpoints
- `GET /MemberManagement/Users` - User list
- `GET /MemberManagement/Users/{id}` - User details
- `POST /MemberManagement/Users/Create` - Create user
- `PUT /MemberManagement/Users/{id}` - Update user
- `DELETE /MemberManagement/Users/{id}` - Delete user

#### DTOs
- `UserListViewModel` - User list data
- `UserDetailViewModel` - User details
- `UserCreateViewModel` - User creation form

### Permission Control Module
**UI Affiliation**: Admin  
**Description**: Role-based access control (RBAC)

#### Features
- Role management
- Permission assignment
- Access control configuration
- Audit logging

#### Routes/Endpoints
- `GET /MemberManagement/Roles` - Role list
- `POST /MemberManagement/Roles/Create` - Create role
- `PUT /MemberManagement/Roles/{id}/Permissions` - Assign permissions

#### DTOs
- `RoleViewModel` - Role information
- `PermissionViewModel` - Permission data

## OnlineStore Area (Public UI)

### Product Management Module
**UI Affiliation**: Public  
**Description**: Product catalog and management

#### Features
- Product catalog browsing
- Product search and filtering
- Product detail views
- Product reviews and ratings

#### Routes/Endpoints
- `GET /OnlineStore` - Store home
- `GET /OnlineStore/Products` - Product list
- `GET /OnlineStore/Products/{id}` - Product details
- `GET /OnlineStore/Search` - Product search

#### DTOs
- `ProductViewModel` - Product display data
- `ProductSearchViewModel` - Search parameters
- `ProductReviewViewModel` - Review information

### Order Processing Module
**UI Affiliation**: Public  
**Description**: Shopping cart and order management

#### Features
- Shopping cart functionality
- Checkout process
- Order tracking
- Payment processing

#### Routes/Endpoints
- `GET /OnlineStore/Cart` - Shopping cart
- `POST /OnlineStore/Cart/Add` - Add to cart
- `POST /OnlineStore/Checkout` - Process checkout
- `GET /OnlineStore/Orders` - Order history

#### DTOs
- `CartViewModel` - Shopping cart data
- `OrderViewModel` - Order information
- `CheckoutViewModel` - Checkout form

### Coupon System Module
**UI Affiliation**: Public  
**Description**: Discount and promotion management

#### Features
- Coupon browsing
- Coupon redemption
- Promotional campaigns
- Discount calculation

#### Routes/Endpoints
- `GET /OnlineStore/Coupons` - Available coupons
- `POST /OnlineStore/Coupons/Redeem` - Redeem coupon
- `GET /OnlineStore/Coupons/MyCoupons` - User coupons

#### DTOs
- `CouponViewModel` - Coupon information
- `CouponRedemptionViewModel` - Redemption data

## Social Hub Area (Mixed UI)

### Message Management Module
**UI Affiliation**: Admin  
**Description**: Message moderation and management

#### Features
- Message list and filtering
- Message moderation tools
- Spam detection
- User communication management

#### Routes/Endpoints
- `GET /social_hub/Messages` - Message list (Admin)
- `POST /social_hub/Messages/Moderate` - Moderate message
- `DELETE /social_hub/Messages/{id}` - Delete message

#### DTOs
- `MessageListViewModel` - Message list data
- `MessageModerationViewModel` - Moderation data

### Chat System Module
**UI Affiliation**: Public  
**Description**: Real-time chat functionality

#### Features
- Real-time messaging
- Contact management
- Message history
- File sharing

#### Routes/Endpoints
- `GET /social_hub/Chat` - Chat interface
- `GET /social_hub/Chat/Contacts` - Contact list
- `POST /social_hub/Chat/Send` - Send message

#### DTOs
- `ChatViewModel` - Chat interface data
- `ContactViewModel` - Contact information
- `MessageViewModel` - Message data

### Notification System Module
**UI Affiliation**: Mixed  
**Description**: System notifications and alerts

#### Features
- Notification creation (Admin)
- Notification display (Public)
- Notification management (Admin)
- User notification preferences

#### Routes/Endpoints
- `GET /social_hub/Notifications` - User notifications
- `POST /social_hub/Notifications/Create` - Create notification (Admin)
- `PUT /social_hub/Notifications/{id}/Read` - Mark as read

#### DTOs
- `NotificationViewModel` - Notification data
- `NotificationCreateViewModel` - Creation form

## API Documentation

### OpenAPI Specification
- Swagger UI available at `/swagger`
- API documentation at `/swagger/v1/swagger.json`
- Interactive API testing interface

### Authentication
- JWT token-based authentication
- Role-based authorization
- API key authentication for external services

### Rate Limiting
- 100 requests per minute per user
- 1000 requests per minute per IP
- Admin endpoints: 500 requests per minute

## Database Schema

### Key Tables
- `Users` - User accounts
- `User_Wallet` - Wallet information
- `Pets` - Pet data
- `Forums` - Forum categories
- `Threads` - Forum threads
- `Posts` - Forum posts
- `Products` - Store products
- `Orders` - Order information
- `Notifications` - System notifications

### Relationships
- Users have one Wallet
- Users can have multiple Pets
- Users can create multiple Threads/Posts
- Users can place multiple Orders
- Users receive multiple Notifications

## Security Considerations

### Data Protection
- All sensitive data encrypted
- Password hashing with bcrypt
- SQL injection prevention
- XSS protection

### Access Control
- Role-based permissions
- Resource-level authorization
- API endpoint protection
- Admin function restrictions

## Performance Considerations

### Caching
- Redis for session storage
- Memory cache for frequently accessed data
- CDN for static assets

### Database Optimization
- Indexed queries
- Connection pooling
- Query optimization
- Read replicas for reporting

## Testing

### Test Coverage
- Unit tests: 85%+ coverage
- Integration tests: All API endpoints
- E2E tests: Critical user flows

### Test Types
- Unit tests for business logic
- Integration tests for API endpoints
- E2E tests for user workflows
- Performance tests for load testing