using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {

     [Authorize(Policy = "RequireAdminRole")]
     [HttpGet("users-with-roles")]

      public ActionResult GetUsersWithRoles()
      {
        //test with postman if policy only allows the admin user
        return Ok("Only admins can see this");
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
