namespace API.Interfaces
{
    public interface ILikesRepository
    {
      Task<AppUserLike> GetAppUserLike(int sourceUserId, int likedUserId);
      Task<PagedList<LikeDTO>> GetUserLikes(LikesParameters likesParams);
      Task<AppUser> GeUserWithLikes(int userId);

    }
}
