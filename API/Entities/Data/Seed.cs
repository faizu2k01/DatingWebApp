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
        public static async Task SeedUsers(DataContext dataContext)
        {
            if (await dataContext.User.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Entities/Data/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach(var user in users)
            {
                using var hmac = new HMACSHA512();

                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));

                user.PasswordSalt = hmac.Key;

                dataContext.User.Add(user);


            }


             await dataContext.SaveChangesAsync();


        } 
    }
}
