using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NachoTacos.Identity.Model.Constants;
using System;

namespace NachoTacos.Identity.EntityFrameworkCore.SqlServer
{
    public static class SeedDefaultAdminRole
    {
        public static void EnsurePopulated(ModelBuilder modelBuilder)
        {
            string ADMIN_ID = Guid.NewGuid().ToString();
            string SYSADMIN_ROLE_ID = Guid.NewGuid().ToString();
            string GUEST_ROLE_ID = Guid.NewGuid().ToString();

            string adminPassword = "superSecretPassword$123";
            string adminEmail = "john.doe@nachotacos.com";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = SYSADMIN_ROLE_ID,
                    Name = RoleType.SYSTEM_ADMIN,
                    NormalizedName = RoleType.SYSTEM_ADMIN.ToUpperInvariant()
                });

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = GUEST_ROLE_ID,
                    Name = RoleType.GUEST,
                    NormalizedName = RoleType.GUEST.ToUpperInvariant()
                });

            var hasher = new PasswordHasher<IdentityUser>();
            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = ADMIN_ID,
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, adminPassword),
                    SecurityStamp = string.Empty
                });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = SYSADMIN_ROLE_ID,
                    UserId = ADMIN_ID
                });
        }
    }
}
