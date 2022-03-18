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


        //Give the table names
        public DbSet<AppUser> User { get; set; }
        public DbSet<AppUserLike> Likes { get; set; }
        public DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
          base.OnModelCreating(builder);
          /*For the Likes table, configure primary key */
          builder.Entity<AppUserLike>()
            .HasKey(k => new{k.SourceUserId, k.LikedUserId});

          /* configure the many to many relationship between
            AppUser and AppUserLike entities and foreign key */

          builder.Entity<AppUserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

          builder.Entity<AppUserLike>()
            .HasOne(x => x.LikedUser)
            .WithMany(m => m.LikedByUsers)
            .HasForeignKey(s => s.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

          builder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(m => m.MesagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

          builder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        }


    }
}
