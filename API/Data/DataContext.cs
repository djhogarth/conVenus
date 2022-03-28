using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    /* Inherits from IdentityDbContext and types are specified so the primiary key is of type int */
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
      AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //Give the table names
        public DbSet<AppUserLike> Likes { get; set; }
        public DbSet<Message> Message { get; set; }

        public DbSet<Group> Groups {get; set;}

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

          base.OnModelCreating(builder);

          /* Configure many-to-many relationship between AppUser
              and Role entities */

          builder.Entity<AppUser>()
            .HasMany(usrRole => usrRole.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(usrRole => usrRole.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(usrRole => usrRole.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(usrRole => usrRole.RoleId)
            .IsRequired();

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

          builder.ApplyUtcDateTimeConverter();

        }
    }

  /* A class that converts all the date times coming out of the database into UTC format. */
  public static class UtcDateAnnotation
  {
    private const String IsUTCAnnotation = "IsUTC";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
      new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
      new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUTC<TProperty>(this PropertyBuilder<TProperty> builder, Boolean IsUTC = true) =>
      builder.HasAnnotation(IsUTCAnnotation, IsUTC);

    public static Boolean IsUTC(this IMutableProperty property) =>
      ((Boolean?)property.FindAnnotation(IsUTCAnnotation)?.Value) ?? true;

  /// <summary>
  /// Make sure this is called after configuring all your entities.
  /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
      foreach (var entityType in builder.Model.GetEntityTypes())
      {
        foreach (var property in entityType.GetProperties())
        {
          if (!property.IsUTC())
          {
            continue;
          }

          if (property.ClrType == typeof(DateTime))
          {
            property.SetValueConverter(UtcConverter);
          }

          if (property.ClrType == typeof(DateTime?))
          {
            property.SetValueConverter(UtcNullableConverter);
          }
        }
      }
    }
  }
}
