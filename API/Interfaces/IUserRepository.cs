using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
      //Allow the user to update their profile
        void UpdateUser (AppUser user);

        // returns a list of users
        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id) ;


        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<string> GetUserGender(string username);

        Task<MemberDTO> GetMemberByUsernameAsync(string username, bool isCurrentUser);

        Task<PagedList<MemberDTO>> GetMembersAsync( UserParameters parameters);

        Task<AppUser> GetUserByPhotoId(int photoId);








    }
}
