using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
    private readonly UserManager<AppUser> _userManager;

    public AdminController(UserManager<AppUser> _userManager)
    {
      this._userManager = _userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]

      /* Get a list of user objects where each user contains the
         user ID, username and role name(s). This method
         is used when the admin edits user roles */
      public async Task<ActionResult> GetUsersWithRoles()
      {

        var users = await _userManager.Users
          .Include(r => r.UserRoles)
          .ThenInclude(r => r.Role)
          .OrderBy(u => u.UserName)
          .Select(u => new
          {
            u.Id,
            Username = u.UserName,
            Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
          }).ToListAsync();

          return Ok(users);
      }

      [Authorize(Policy = "ModeratePhotoRole")]
      [HttpGet("photos-to-moderate")]

      public ActionResult GetPhotosForModeration()
      {
        //test with postman if policy only allows user with admin and moderator roles
        return Ok("Admins or moderators can see this");
      }

    }
}
