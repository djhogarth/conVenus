
var builder = WebApplication.CreateBuilder(args);

//add services to the container

builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddControllers();
//Add support for Cross Origin Resource Sharing (CORS) between the API and client side
builder.Services.AddCors();
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddSignalR();

//get access to the app
var app = builder.Build();
// configure the HTTP request pipeline
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

//Add a CORS policy to allow access to API resources from the origin located at localhost:4200
app.UseCors(policy => policy
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowCredentials()
  .WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index","Fallback");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using var scope = app.Services.CreateScope();
//create a scope for the services
var services = scope.ServiceProvider;
try
{
  /* we want to get our DataContext service so we can pass it
  into our Seed method */
  var context = services.GetRequiredService<DataContext>();
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
  await context.Database.MigrateAsync();
  await Seed.SeedUsers(userManager, roleManager);
}
catch(Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error has occured during migration");
}

await app.RunAsync();
