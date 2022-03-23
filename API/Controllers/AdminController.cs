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

      [HttpPost("edit-roles/{username}")]

      public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
      {
        /* The roles input will be a comma separtated list.
           So we get the roles by splitting them by the comma */
        var selectedRoles = roles.Split(",").ToArray();

        /* Find user by username and return not found if user doesn't exist */
        var user = await _userManager.FindByNameAsync(username);

        if(user == null) return NotFound("Could not find user");

        /* Get given roles for the particular user */
        var userRoles = await _userManager.GetRolesAsync(user);

        /* look at the selected roles to be edited and add the user to the roles
           unless they're in that particular role */

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if(!result.Succeeded) return BadRequest("Failed to add to roles");

        /* get rid of roles the user had previously before the edit */
        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if(!result.Succeeded) return BadRequest("Failed to remove from roles");

        //return edited roles and save the changes
        return Ok(await _userManager.GetRolesAsync(user));
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
