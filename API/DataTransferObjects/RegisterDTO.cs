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
        [StringLength(8, MinimumLength = 4)]
        public String Password { get; set; }

        
        
    }
}