using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Interfaces;

namespace API.Data
{
  public class LikesRepository : ILikesRepository
  {
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
      _context = context;
    }

    public Task<AppUserLike> GetAppUserLike(int sourceUserId, int likedUserId)
    {
      throw new NotImplementedException();
    }

    public Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId)
    {
      throw new NotImplementedException();
    }

    public Task<AppUser> GeUserWithLikes(int userId)
    {
      throw new NotImplementedException();
    }
  }
}
