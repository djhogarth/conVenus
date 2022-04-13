
namespace API.Services
{
    public class TokenService : ITokenService
    {
      private readonly SymmetricSecurityKey _key;
      private readonly UserManager<AppUser> _userManager;

      public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
          _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["TokenKey"]));
          _userManager = userManager;
        }

      public async Task<string> CreateToken(AppUser user)
      {
        //adding our claims
        var claims = new List<Claim>
        {
          //nameId of our Jwt token will be the user's username
          new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        //list of roles belonging to the user
        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        /*
        Create some credentials. We specify what security algorithm
        to use for encrypting our jwt token and the security key.
        */
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        //Specify what goes inside our token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(claims),
          //token expires seven days from now
          Expires = DateTime.UtcNow.AddDays(7),
          SigningCredentials = credentials
        };

        /*
        create token handler which is used to create
        and write the token
          */
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        //return writen token to whoever needs it
        return tokenHandler.WriteToken(token);
    }
  }
}
