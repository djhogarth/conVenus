using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    //Derive from DbContext class which we get from the Entity Framwork Core package 
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }


        //Create a DbSet for class AppUser and create a corresponding table inside our Database called Users. 
        public DbSet<AppUser> Users { get; set; }
    }
}