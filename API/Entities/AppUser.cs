using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        public int ID { get; set; } 

        public string UserName { get; set; }
        
        public byte[] PasswordSalt {get; set; }

        
        public byte[] PasswordHash { get; set; }
    }
}