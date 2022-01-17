using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}