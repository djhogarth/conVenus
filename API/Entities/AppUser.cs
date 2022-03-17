using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

namespace API.Entities
{

    public class AppUser
    {
        // userID
        public int ID { get; set; }

        public string UserName { get; set; }

        /*  password salts prevent two users with the same password from getting the same hash.
            It also adds another layer of complexity to the password hash   */
        public byte[] PasswordSalt {get; set; }

        //password hashing is used to ecrypt passwords stored in the database
        public byte[] PasswordHash { get; set; }

        public DateTime DateOfBirth{get; set;}

        //The name the user wants to be known as
        public string Alias { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Gender { get; set; }

        public String Introduction  { get; set; }
        public string LookingFor { get; set; }

        public string Interests {get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }

        //List of users who liked the logged in user
        public ICollection<AppUserLike> LikedByUsers { get; set; }
        //List of users who are liked by the the logged in user
        public ICollection<AppUserLike> LikedUsers { get; set; }
    }
}
