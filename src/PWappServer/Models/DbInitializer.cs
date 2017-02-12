using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer.Models
{
    public static class DbInitializer
    {
        public static async void InitializeUser(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Looks for any users.
            /*
            if (context.Users.Any())
            {
                return; // DB has been seeded.
            }
            */

            // Seeds an administrator user.
            var user = new ApplicationUser
            {
             
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "user@gmail.com",
                EmailConfirmed = false,
                LockoutEnabled = true,
                NormalizedEmail = "user@GMAIL.COM",
                NormalizedUserName = "user@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                UserName = "user@gmail.com",
                Amount = 500
                //Books = new List<Book> { new Book() { Name = "Dostoevsky" }, new Book() { Name = "Tolstoy" }, }

            };

            // Password.
            var password = new PasswordHasher<ApplicationUser>();
            var passawordHashed = password.HashPassword(user, "Admin01*");

            user.PasswordHash = passawordHashed;

            // Claims.
            var claims = new IdentityUserClaim<string>[] {
               
                new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = "user" }
            };
            foreach (var claim in claims)
            {
                user.Claims.Add(claim);
            }


            var userStore = new UserStore<ApplicationUser>(context);
            await userStore.CreateAsync(user);

            await context.SaveChangesAsync();
        }




        public static async void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Looks for any users.
            /*
            if (context.Users.Any())
            {
                return; // DB has been seeded.
            }
            */

            // Seeds an administrator user.
            var user = new ApplicationUser
            {

                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "admin@gmail.com",
                EmailConfirmed = false,
                LockoutEnabled = true,
                NormalizedEmail = "ADMIN@GMAIL.COM",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                UserName = "admin@gmail.com",
                Name = "AdminName",
                Amount = 500
                //Books = new List<Book> { new Book() { Name = "Dostoevsky" }, new Book() { Name = "Tolstoy" }, }

            };

            // Password.
            var password = new PasswordHasher<ApplicationUser>();
            var passawordHashed = password.HashPassword(user, "Admin01*");

            user.PasswordHash = passawordHashed;

            // Claims.
            var claims = new IdentityUserClaim<string>[] {
                new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = "administrator" }
            };
            foreach (var claim in claims)
            {
                user.Claims.Add(claim);
            }


            var userStore = new UserStore<ApplicationUser>(context);


            ////


            var user2 = new ApplicationUser
            {
              
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "user@gmail.com",
                EmailConfirmed = false,
                LockoutEnabled = true,
                NormalizedEmail = "user@GMAIL.COM",
                NormalizedUserName = "user@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                UserName = "user@gmail.com",
                Amount = 500
                //Books = new List<Book> { new Book() { Name = "Dostoevsky" }, new Book() { Name = "Tolstoy" }, }

            };

            // Password.
            var password2 = new PasswordHasher<ApplicationUser>();
            var passawordHashed2 = password.HashPassword(user2, "Admin01*");

            user2.PasswordHash = passawordHashed;

            // Claims.
            var claims2 = new IdentityUserClaim<string>[] {
               
                new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = "user" }
            };
            foreach (var claim in claims2)
            {
                user2.Claims.Add(claim);
            }


            var userStore2 = new UserStore<ApplicationUser>(context);
            await userStore2.CreateAsync(user2);

            await context.SaveChangesAsync();




            ///////////


            await userStore.CreateAsync(user);

            await context.SaveChangesAsync();
        }
    }
}
