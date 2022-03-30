namespace API.Entities
{
  public class Photo
  {
    public int Id { get; set; }

    // Photo Url
    public string Url { get; set; }

    // Whether this photo is the main one
    public bool IsMain { get; set; }

    // Whether the photo is approved by the admin or moderator 
    public bool IsApproved { get; set; }
    public AppUser User { get; set; }
    public int UserId { get; set; }

    // Used for the photo storage solution that's used.
    public string PublicId { get; set; }
  }
}
