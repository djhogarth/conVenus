using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DataTransferObjects;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // A class that allows access to our dabase via the DbContext using the _context attribute

    [Authorize]
        public class UsersController : BaseApiController
    {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
      {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _photoService = photoService;
    }

        // An endpoint that allows the retreival of all users in our database.
        //Code is made asynchronous to make application more scalable and able to handle
        // a potential large number of requests to database


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParameters parameters)
        {

          var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
          parameters.CurrentUsername = User.GetUsername();

          if(string.IsNullOrEmpty(parameters.Gender)){

            /*if set the gender parameter to the opposite of
              gender of the current user */

            if( gender == "male") parameters.Gender = "female";
            if( gender == "female") parameters.Gender = "male";

          }
          var users = await _unitOfWork.UserRepository.GetMembersAsync(parameters);

          Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        //An endpoint that allows the retreival of one user via the primaary ID in the database.

         [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username){
          var currentUsername = User.GetUsername();
          var isCurrentUser = (currentUsername == username);

          return await _unitOfWork.UserRepository.GetMemberByUsernameAsync(username, isCurrentUser) ;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){
          var username = User.GetUsername();
          var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

          _mapper.Map(memberUpdateDTO, user);

          _unitOfWork.UserRepository.UpdateUser(user);

          if (await _unitOfWork.Complete()) return NoContent();

          return BadRequest("Failed to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
          //get user through their username
          var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
          //get reponse from photo service whether photo was added successfully
          var result = await this._photoService.AddPhotoAsync(file);
          //return a bad request if adding photo failed
          if(result.Error != null) return BadRequest(result.Error.Message);
          /*create new photo if successful and make it the main
           photo if there's no other photos*/
          var photo = new Photo
          {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
          };

          //add and return photo object
          user.Photos.Add(photo);

          if(await _unitOfWork.Complete()){
            return CreatedAtRoute("GetUser", new{username = user.UserName}, _mapper.Map<PhotoDTO>(photo));
          }
          return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
          var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

          var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

          if(photo.IsMain) return BadRequest("This is already your main photo");

          var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
          if(currentMain != null) currentMain.IsMain = false;
          photo.IsMain = true;

          if(await _unitOfWork.Complete()) return NoContent();

          return BadRequest("Failed to set main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId){
          var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
          var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

          if(photo == null) return NotFound();

          if(photo.IsMain) return BadRequest("You cannot delete your main photo");

          if(photo.PublicId != null)
          {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
          }

          user.Photos.Remove(photo);

          if(await _unitOfWork.Complete()) return Ok();

          return BadRequest("Failed to delete the photo");
        }
    }
}
