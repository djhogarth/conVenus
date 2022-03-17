using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
          if (await context.User.AnyAsync()) return;

          var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
          var users = System.Text.Json.JsonSerializer.Deserialize<List<AppUser>>(userData);
          foreach (var user in users)
          {
            using var hmac = new HMACSHA512();

            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("!QSXDR%"));
            user.PasswordSalt = hmac.Key;

            context.User.Add(user);
          }

          await context.SaveChangesAsync();

        }
    }
}
