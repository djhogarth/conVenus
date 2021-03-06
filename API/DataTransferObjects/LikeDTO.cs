namespace API.DataTransferObjects
{
    public class LikeDTO
    {

      /*the users liked will display as member cards,
        so we only need properties used in member card */
        public int Id {set; get;}

        public string Username { get; set; }

        public int Age { get; set; }

        public string Alias { get; set; }

        public string PhotoUrl {get; set;}

        public string City { get; set; }


    }
}
