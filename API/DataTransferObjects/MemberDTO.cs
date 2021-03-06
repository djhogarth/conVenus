namespace API.DataTransferObjects
{
    public class MemberDTO
    {
        public int ID { get; set; }

        public string UserName { get; set; }
        public string PhotoUrl { get; set; }

        public int Age {get; set;}

        //The name the user wants to be known as
        public string Alias { get; set; }
        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string Introduction  { get; set; }
        public string LookingFor { get; set; }

        public string Interests {get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<PhotoDTO> Photos { get; set; }

    }
}
