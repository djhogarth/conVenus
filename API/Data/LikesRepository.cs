using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class LikesRepository : ILikesRepository
  {
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<AppUserLike> GetAppUserLike(int sourceUserId, int likedUserId)
    {
      return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    /* User can choose between two lists. One containing users
       that they liked. And the other vice versa. The predicate
       variable is used for picking between the two. Both lists
       are defined in the AppUser class.

       The userId is used to compare against the either the
       SourceUserId or the LikedUserId within the Likes table.
    */
    public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParameters likesParams)
    {
      var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
      var likes = _context.Likes.AsQueryable();

      // check which list the user is looking for.
      if(likesParams.Predicate == "liked")
      {
        likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
        //gives the LikedUsers list
        users = likes.Select(like => like.LikedUser);
      }

      //get users that are likedB
      if(likesParams.Predicate == "likedBy")
      {
        likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
        //gives the LikedByUsers list.
        users = likes.Select(like => like.SourceUser);
      }

      var likedUsers = users.Select(user => new LikeDTO{
        Username = user.UserName,
        Alias = user.Alias,
        Age = user.DateOfBirth.CalculateAge(),
        PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
        City = user.City,
        Id = user.Id
      });

      return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GeUserWithLikes(int userId)
    {
      return await _context.Users
        .Include(x => x.LikedUsers)
        .FirstOrDefaultAsync(x => x.Id == userId);

    }
  }
}
