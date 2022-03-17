using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /* The AccountController class implements the register and login actions of the AppUser */
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    // inject the DbContext and the token service into the account controller
    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
      {
        _tokenService = tokenService;
        _mapper = mapper;
        _context = context;
      }

        //this endpoint adds new users to our database

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO registerDTO){
             if(await UserExists(registerDTO.UserName)) return BadRequest("Username already exists") ;

            using var hmac = new HMACSHA512();

            var user = _mapper.Map<AppUser>(registerDTO);

            //password salt is set to be the randomly generated key that was created when hmac was initialized

                user.UserName = registerDTO.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                user.PasswordSalt = hmac.Key;



            //add new user entity to the DbContext to track changes
            _context.User.Add(user);

            //save new user to database
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
              Alias = user.Alias,
                Gender = user.Gender
            };

        }

        /*this endpoint checks if a user exists in the database
         and if the password is correct. If either is false then
         return an error message */

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> LoginUser(LoginDTO loginDTO){

            //get user from database
            var user = await _context.User
            .Include(u => u.Photos).
            SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

             // return error message if username is not found
            if (user == null) return Unauthorized("Invalid Username!");
             /*HMACSHA512 provides the hashing algorithm used to calculate the computed
             password hash using the user's password salt from the database. */

            using var hmac = new HMACSHA512(user.PasswordSalt);

            //compute the password hash of the password inputed using the LoiginDTO
            var computedPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            //compare to see if both hashes are equal in order to verify matching passwords. If not return error message.
            for(int i=0;i<computedPasswordHash.Length;i++){
                 if (computedPasswordHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                Alias = user.Alias,
                Gender = user.Gender
            };

        }

        //this method checks if the database contains a specific user for a given username
         private async  Task<bool> UserExists (String Username) {
             return await _context.User.AnyAsync(x => x.UserName == Username.ToLower());
          }


    }
}
