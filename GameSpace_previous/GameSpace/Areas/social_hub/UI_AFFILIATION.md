# Social Hub Area - UI Affiliation Declaration

**Area**: social_hub  
**UI Affiliation**: Mixed (Admin + Public)  
**Layout**: Areas/social_hub/Views/Shared/_Layout.cshtml  
**Framework**: Mixed (SB Admin for Admin, Bootstrap for Public)  

## Module UI Affiliations

1. **Message Management** - Admin UI
   - Message list interface
   - Message moderation interface
   - Message analytics interface

2. **Chat System** - Public UI
   - Chat interface
   - Contact management interface
   - Message history interface

3. **Notification System** - Mixed UI
   - Admin: Notification creation interface
   - Public: Notification display interface
   - Admin: Notification management interface

4. **Mute Filter** - Admin UI
   - Mute word management interface
   - Filter configuration interface
   - Filter analytics interface

## Design Guidelines

- Admin modules: Use SB Admin components
- Public modules: Use Bootstrap components
- Maintain clear separation between Admin/Public
- Extract sidebar/topbar into Area-level partials for Admin

## Prohibited

- Do not mix Admin/Public components in same view
- Do not edit vendor libraries
- Do not use wrong framework for module type