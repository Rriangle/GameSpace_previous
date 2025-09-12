# MemberManagement Area - UI Affiliation Declaration

**Area**: MemberManagement  
**UI Affiliation**: Admin  
**Layout**: Areas/MemberManagement/Views/Shared/_Layout.cshtml  
**Framework**: SB Admin (Admin back office)  

## Module UI Affiliations

1. **User Management** - Admin UI
   - User list interface
   - User detail view
   - User creation/editing interface

2. **Permission Control** - Admin UI
   - Role management interface
   - Permission assignment interface
   - Access control interface

3. **User Analytics** - Admin UI
   - User statistics dashboard
   - Activity monitoring interface
   - Report generation interface

## Design Guidelines

- Use SB Admin components (third-party library)
- Do not modify vendor files
- Extract sidebar/topbar into Area-level partials
- Maintain admin-specific styling

## Prohibited

- Do not use Bootstrap public components
- Do not mix Admin/Public assets
- Do not edit vendor libraries