using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /* The AccountController class implements the register and login actions of the AppUser */
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        // gives the ActionController access to the datbase through the DbContext via the _context attribute
        public AccountController(DataContext context)
        {
            _context = context;
        }

        //this endpoint adds new users to our database
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> RegisterUser(RegisterDTO registerDTO){
             if(await UserExists(registerDTO.UserName)) return BadRequest("Username already exists") ;

            using var hmac = new HMACSHA512();

            //password salt is set to be the randomly generated key that was created when hmac was initialized
            var user = new AppUser{
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            //add new user entity to the DbContext to track changes
            _context.Users.Add(user);

            //save new user to database
            await _context.SaveChangesAsync();
            return user;
                        
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> LoginUser(LoginDTO loginDTO){
             
            //get user from database
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

             // return error message if username is not found
            if (user == null) return Unauthorized("Invalid Username! Please register or re-type your Username.");
             /*HMACSHA512 provides the hashing algorithm used to calculate the computed 
             password hash using the user's password salt from the database. */
            
            using var hmac = new HMACSHA512(user.PasswordSalt);

            //compute the password hash of the password inputed using the LoiginDTO
            var computedPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            //compare to see if both hashes are equal in order to verify matching passwords. If not return error message.
            for(int i=0;i<computedPasswordHash.Length;i++){
                 if (computedPasswordHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }
            
            return user;
            
        }
        
        //check if database contains a specific user given a username
         private async  Task<bool> UserExists (String Username) { 
             return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());
          }

        
    }
}