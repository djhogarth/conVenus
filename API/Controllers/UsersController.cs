using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // A class that allows access to our dabase via the DbContext using the _context attribute

    [Authorize]
        public class UsersController : BaseApiController
    {
      private readonly IUserRepository _userRepository;

      public UsersController(IUserRepository userRepository)
      {
      _userRepository = userRepository;
      }

        // An endpoint that allows the retreival of all users in our database.
        //Code is made asynchronous to make application more scalable and able to handle
        // a potential large number of requests to database


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){


            return Ok(await _userRepository.GetUsersAsync());
;
        }

        //An endpoint that allows the retreival of one user via the primaary ID in the database.


         [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUser(string username){


            return await _userRepository.GetUserByUsernameAsync(username);
        }
    }
}
