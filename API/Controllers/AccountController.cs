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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /* The AccountController class implements the register and login actions of the AppUser */
    public class AccountController : BaseApiController
    {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    // inject the DbContext and the token service into the account controller
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
      {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
      }

        //this endpoint adds new users to the database

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO registerDTO){
             if(await UserExists(registerDTO.UserName)) return BadRequest("Username already exists") ;

            var user = _mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.UserName.ToLower();

            //add a new user to database and save changes.
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

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
            var user = await _userManager.Users
            .Include(u => u.Photos).
            SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName.ToLower());

             // return error message if username is not found
            if (user == null) return Unauthorized("Invalid Username!");

            var result = await _signInManager
              .CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!result.Succeeded) return Unauthorized();

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
             return await _userManager.Users.AnyAsync(x => x.UserName == Username.ToLower());
          }


    }
}
