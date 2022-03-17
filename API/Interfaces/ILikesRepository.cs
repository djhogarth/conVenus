using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
      Task<AppUserLike> GetAppUserLike(int sourceUserId, int likedUserId);
      Task<PagedList<LikeDTO>> GetUserLikes(LikesParameters likesParams);
      Task<AppUser> GeUserWithLikes(int userId);

    }
}
