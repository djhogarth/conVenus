namespace API.Entities
{

    public class AppUser : IdentityUser<int>
    {
        public DateOnly DateOfBirth{get; set;}

        //The name the user wants to be known as
        public string Alias { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

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
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MesagesReceived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
