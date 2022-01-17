using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // A class that allows access to our dabase via the DbContext using the _context attribute 
        public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        // An endpoint that allows the retreival of all users in our database. 
        //Code is made asynchronous to make application more scalable and able to handle
        // a potential large number of requests to database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
           
            return await  _context.Users.ToListAsync();
;
        }

        //An endpoint that allows the retreival of one user via the primaary ID in the database. 
        
         [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id){
            

            return await  _context.Users.FindAsync(id);;
        }
    }
}