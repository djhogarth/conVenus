using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDTO>> GetUnApprovedPhotos();

        Task<Photo> GetPhotoById(int photoId);

        void RemovePhoto(Photo picture);
    }
}
