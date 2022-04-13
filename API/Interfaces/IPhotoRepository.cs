namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDTO>> GetUnApprovedPhotos();

        Task<Photo> GetPhotoById(int photoId);

        void RemovePhoto(Photo picture);
    }
}
