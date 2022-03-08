namespace API.Entities
{
  public class Photo
  {
    public int Id { get; set; }

    //photo Url
    public string Url { get; set; }
    public bool IsMain { get; set; }

    public AppUser User { get; set; }
    public int UserId { get; set; }

//used for the photo storage solution that's used
    public string PublicId { get; set; }
  }
}
