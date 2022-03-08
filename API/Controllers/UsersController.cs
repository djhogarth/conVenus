using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Interfaces;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
      {
      _userRepository = userRepository;
      _mapper = mapper;
    }

        // An endpoint that allows the retreival of all users in our database.
        //Code is made asynchronous to make application more scalable and able to handle
        // a potential large number of requests to database


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
          var users = await _userRepository.GetMembersAsync();

            return Ok(users);

        }

        //An endpoint that allows the retreival of one user via the primaary ID in the database.


         [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username){

          return await _userRepository.GetMemberByUsernameAsync(username) ;
        }
    }
}
