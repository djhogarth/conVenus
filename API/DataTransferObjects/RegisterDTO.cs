namespace API.DataTransferObjects
{
    public class RegisterDTO
    {
        [Required] public String UserName { get; set; }

        [Required] public string Alias { get; set; }

        [Required] public string Gender { get; set; }

        [Required] public DateTime DateOfBirth { get; set; }

        [Required] public string city { get; set; }

        [Required] public string country { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4)]
        public String Password { get; set; }



    }
}
