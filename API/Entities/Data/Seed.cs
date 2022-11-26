using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Entities/Data/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
                new AppRole{Name = "Member"}
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
                user.UserName = user.UserName.ToLower();

                 await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

            }

            var admin = new AppUser
            {
                UserName = "admin",

                Gender = "male",
                DateOfBirth = DateTime.Today,
                KnownAs= "Faizu",
                Created =  DateTime.Now,
                Introduction= "Anim aute occaecat occaecat consectetur non. Consectetur esse non ut velit ut Lorem culpa eiusmod. Lorem ex consectetur incididunt culpa quis anim laboris minim cupidatat magna aliquip. Fugiat elit ad fugiat amet nostrud duis velit tempor Lorem nisi ea nisi cupidatat. Exercitation velit adipisicing excepteur pariatur. Proident commodo ex culpa qui.\r\n",
                LookingFor =  "Quis Lorem velit sunt deserunt do id dolore proident commodo qui duis ut. Culpa occaecat consectetur proident culpa id velit esse deserunt nostrud cupidatat nulla anim ipsum non. Cupidatat ut aliquip duis dolor exercitation. Ex aliquip ea exercitation culpa laborum. Tempor laborum aute cillum anim veniam fugiat nisi eu voluptate eu ea dolore velit culpa. Cillum nostrud enim officia ut. Esse veniam et reprehenderit amet officia in culpa qui laboris enim quis exercitation eiusmod reprehenderit.\r\n",
                Interests= "Mollit qui consequat enim exercitation.",
                City= "Volta",
                Country =  "United Kingdom",
              };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin,new[] { "Admin","Moderator"});
        } 
    }
}
