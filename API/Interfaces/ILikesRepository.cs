using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
      Task<AppUserLike> GetAppUserLike(int sourceUserId, int likedUserId);
      Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId);
      Task<AppUser> GeUserWithLikes(int userId);

      
    }
}
