namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
    {
      services.AddSingleton<PresenceTracker>();
      services.AddScoped<IPhotoService, PhotoService>();
      services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
      services.AddScoped<UpdateUserActivity>();
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
      services.AddDbContext<DataContext>(options =>
      {
        // configure the DataContext to connect to a Microsoft SQL Server database and set the connection string.
        options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

      });

      return services;
    }
  }
}
