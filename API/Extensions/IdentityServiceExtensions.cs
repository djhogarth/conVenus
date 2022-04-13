namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityService (this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
              opt.Password.RequireNonAlphanumeric = false;
            }).AddRoles<AppRole>()
              .AddRoleManager<RoleManager<AppRole>>()
              .AddSignInManager<SignInManager<AppUser>>()
              .AddRoleValidator<RoleValidator<AppRole>>()
              .AddEntityFrameworkStores<DataContext>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, //validate that the server signed key is correct
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false, //Our API
                    ValidateAudience = false //Our Angular app

                };

                options.Events = new JwtBearerEvents
                {
                  //Context is the http request.
                  OnMessageReceived = context =>
                  {

                  /* Allow the client to send the token as a query string.
                     SignalR by default, sends up the token as a query string
                     with a key of 'access_token' */
                    var accessToken = context.Request.Query["access_token"];

                    //get the path of this http request
                    var path = context.HttpContext.Request.Path;

                    //check if we got the Jwt token and that the path is for SignalR
                    if(!string.IsNullOrEmpty(accessToken) &&
                      path.StartsWithSegments("/hubs"))
                    {
                      context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                  }
                };
            });

            // create authorization policies
            services.AddAuthorization(opt =>
            {
              opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
              opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

             return services;

        }
    }
}
