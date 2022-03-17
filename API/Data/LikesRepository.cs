using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Extensions;
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
    public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId)
    {
      var users = _context.User.OrderBy(u => u.UserName).AsQueryable();
      var likes = _context.Likes.AsQueryable();

      // check which list the user is looking for.
      if(predicate == "liked")
      {
        likes = likes.Where(like => like.SourceUserId == userId);
        //gives the LikedUsers list
        users = likes.Select(like => like.LikedUser);
      }

      //get users that are likedB
      if(predicate == "likedBy")
      {
        likes = likes.Where(like => like.LikedUserId == userId);
        //gives the LikedByUsers list.
        users = likes.Select(like => like.SourceUser);
      }

      return await users.Select(user => new LikeDTO{
        Username = user.UserName,
        Alias = user.Alias,
        Age = user.DateOfBirth.CalculateAge(),
        PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
        City = user.City,
        Id = user.ID
      }).ToListAsync();
    }

    public async Task<AppUser> GeUserWithLikes(int userId)
    {
      return await _context.User
        .Include(x => x.LikedUsers)
        .FirstOrDefaultAsync(x => x.ID == userId);

    }
  }
}
