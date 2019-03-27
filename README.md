# AspNetCoreMultiTenantIdentity

(Or ASP.NET Core Multi-Tenant Identity)

Long name for a simple purpose: add multi-tenant support to [ASP.NET Core](https://github.com/aspnet/AspNetCore) Identity.

## Requirements

This project is built on .NET Standard 2.0. It currently works with ASP.NET Core Identity version 2.1.

## The Basics

### Installation

To get the basics, install the core package from NuGet:

    Install-Package Eleven41.AspNetCore.MultiTenantIdentity

### Setup

1. Add `using` directives where required:

```csharp
using Eleven41.AspNetCore.MultiTenantIdentity;
```

2. Derive your user class from `MultiTenantUser` rather than `IdentityUser`:

```csharp
class ApplicationUser : MultiTenantUser
{
}
```

You can also derive from `MultiTenantUser<>` to indicate the key type.

The `MultiTenantUser` class adds a `TenantId` member to your users to indicate the tenant they are a member of. This new property is *required*. So it must be set on your user before the user is added to your store.

Please see the samples for a registration example.

3. If you are also using roles, derive your role class from `MultiTenantRole` rather than `IdentityRole`:

```csharp
class ApplicationRole : MultiTenantRole
{
}
```

Similarly, you can template the `MultiTenantRole<>` with the key type.

4. This package introduces a new class called `IdentityTenant` that represents the tenants (groups, organizations, companies) that your users are contained within. This class can be templated using `IdentityTenant<>` to indicate the key type.

Note: Key types should be the same between users, roles, and tenants.

5. In `Startup.cs`, install the core services after your 

```csharp
services.AddDefaultIdentity<ApplicationUser>()
    .AddMultiTenantIdentity<ApplicationTenant>()
    ;
```

This will add `TenantManager<ApplicationUser, ApplicationTenant>` into the dependency injection services.

### Usage

In your controllers, access the tenant manager as follows:

```csharp
class HomeController
{
    TenantManager<ApplicationUser, ApplicationTenant> _tenantManager;

    public HomeController(TenantManager<ApplicationUser, ApplicationTenant> tenantManager)
    {
        _tenantManager = tenantManager ?? throw new ArgumentNullException(nameof(tenantManager));
    }

    public async Task<IActionResult> Index()
    {
        if (this.User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var tenant = await _tenantManager.FindByIdAsync(user.TenantId.ToString());
            ViewData["TenantName"] = await _tenantManager.GetNameAsync(tenant);
        }
        return View();
    }
}
```

## Entity Framework Core Usage

### Installation

If you are using Entity Framework Core for your identity storage, you should also install the Entity Framework middleware:

    Install-Package Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore

### Setup

1. Add `using` directives where required:

```csharp
using Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore;
```

2. Derive your Identity context from `MultiTenantIdentityDbContext` (if you are using roles) or `MultiTenantIdentityUserContext` if you are not using roles. Basically, just put `MultiTenant` at the start of the base class you're using.

```csharp
public class ApplicationIdentityContext : MultiTenantIdentityUserContext<ApplicationUser, ApplicationTenant> 
{
    public ApplicationIdentityContext(DbContextOptions<ApplicationIdentityContext> options)
            : base(options)
    {
    }
}
```

3. In `Startup.cs`, call `AddMultiTenantEntityFrameworkStores` with your identity context class.

```csharp
services.AddDefaultIdentity<ApplicationUser>()
    .AddEntityFrameworkStores<IdentityContext>()

    .AddMultiTenantIdentity<ApplicationTenant>()
    .AddMultiTenantEntityFrameworkStores<ApplicationIdentityContext>()
    ;
```

4. Add a database migration.

    dotnet ef migrations add AddMultiTenancy

5. Update your database.

    dotnet ef database update

At this point, you should see 2 things in your database:

* A new table was added called `AspNetTenants`.
* The `AspNetUsers` table should have a new column called `TenantId` with a foreign key pointing to `AspNetTenants`.

## Current Functionality

* User names and emails must still follow the normal Identity uniqueness requirements. Users are not isolated within tenants.
* Adding multi-tenancy to an existing database is currently problematic since the `TenantId` added to users is non-null and required.

## Goals

* Add user management pages to add to your MVC applications to add/remove users within a tenant.
* Add optional user isolation to allow the same username and email in multiple tenants.
