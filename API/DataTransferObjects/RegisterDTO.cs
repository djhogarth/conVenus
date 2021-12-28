using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataTransferObjects
{
    public class RegisterDTO
    {
        [Required]
        public String UserName { get; set; }

        [Required]
        public String Password { get; set; }

        
        
    }
}