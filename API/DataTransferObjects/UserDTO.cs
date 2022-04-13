namespace API.DataTransferObjects
{ // This is the object that we return when there's a user log-in
    public class UserDTO
    {
        public string Username { get; set; }

        public string Token { get; set; }

        public string PhotoUrl { get; set; }

        public string Alias { get; set; }

        public string Gender { get; set; }


    }
}
