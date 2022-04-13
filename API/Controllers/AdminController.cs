namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;

    public AdminController(UserManager<AppUser> _userManager,
      IUnitOfWork unitOfWork, IPhotoService photoService)
    {
      this._userManager = _userManager;
      _unitOfWork = unitOfWork;
      _photoService = photoService;
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

      public async Task<ActionResult> GetPhotosForApproval()
      {
       var photos = await _unitOfWork.PhotoRepository.GetUnApprovedPhotos();
       return Ok(photos);
      }

      [Authorize(Policy = "ModeratePhotoRole")]
      [HttpPost("approve-photo/{photoId}")]
      public async Task<ActionResult> ApprovePhoto(int photoId)
      {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if(photo == null) return NotFound("Photo could not be found");

        photo.IsApproved = true;

        var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);

        if(!user.Photos.Any(x => x.IsMain))
        {
          photo.IsMain = true;
        }

        await _unitOfWork.Complete();

        return Ok();

      }

      [Authorize(Policy = "ModeratePhotoRole")]
      [HttpPost ("reject-photo/{photoId}")]
      public async Task<ActionResult> RejectPhoto(int photoId)
      {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if(photo == null) return NotFound("Photo could not be found");

        if (photo.PublicId != null)
        {
          var result = await _photoService.DeletePhotoAsync(photo.PublicId);

          if (result.Result == "ok")
          {
            _unitOfWork.PhotoRepository.RemovePhoto(photo);
          }
        }
        else
        {
          _unitOfWork.PhotoRepository.RemovePhoto(photo);
        }

        await _unitOfWork.Complete();

        return Ok();




      }



    }
}
