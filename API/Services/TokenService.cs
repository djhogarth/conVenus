using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            //adding our claims
            var claims = new List<Claim>
            {
                //nameId of our Jwt token will be the user's username
                new Claim(JwtRegisteredClaimNames.NameId, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)

            };

            /*
            creating some credentials. We specify what security algorithm
            to use for encrypting our jwt token and the security key.
            */
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //Specify what goes inside our token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //token expires seven days from now
                Expires = DateTime.Now.AddDays(7),
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
