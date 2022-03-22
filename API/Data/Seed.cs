using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
          if (await userManager.Users.AnyAsync()) return;

          //read in the users seed data from the json file and add them to an empty list of users
          var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
          var users = System.Text.Json.JsonSerializer.Deserialize<List<AppUser>>(userData);

          //check to make sure there are no users
          if(users == null) return;

          //Create roles and add them to the database
          var roles = new List<AppRole>()
          {
            new AppRole{Name = "Admin"},
            new AppRole{Name = "Moderator"},
            new AppRole{Name = "Member"},

          };

          foreach (var role in roles)
          {
            await roleManager.CreateAsync(role);
          }

          //Add the users to the database and assign each a role of 'Member'
          foreach (var user in users)
          {
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
          }

          //Create and add a user admin and assign roles 'Admin' and 'Moderator'
          var admin = new AppUser
          {
            UserName = "admin",

          };

          await userManager.CreateAsync(admin, "Pa$$w0rd");
          await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
    }
  }

}
