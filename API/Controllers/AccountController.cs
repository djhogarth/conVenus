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

        //this endpoint adds new users to the database

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO registerDTO){
             if(await UserExists(registerDTO.UserName)) return BadRequest("Username already exists") ;

            var user = _mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.UserName.ToLower();

            //add new user entity to the DbContext to track changes
            _context.Users.Add(user);

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
            var user = await _context.Users
            .Include(u => u.Photos).
            SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

             // return error message if username is not found
            if (user == null) return Unauthorized("Invalid Username!");


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
             return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());
          }


    }
}
