namespace API.DataTransferObjects
{
  public class PhotoDTO
  {
    public int Id { get; set; }

    //photo Url
    public string Url { get; set; }
    public bool IsMain{ get; set; }
  }
}
