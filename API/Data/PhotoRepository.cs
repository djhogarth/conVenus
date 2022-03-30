using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class PhotoRepository : IPhotoRepository
  {
    private readonly DataContext _context;

    public PhotoRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<Photo> GetPhotoById(int photoId)
    {
      return await _context.Photos
        .IgnoreQueryFilters()
        .SingleOrDefaultAsync(x => x.Id == photoId);
    }

    public async Task<IEnumerable<PhotoForApprovalDTO>> GetUnApprovedPhotos()
    {
       var photos = await  _context.Photos
        .IgnoreQueryFilters()
        .Where(x => x.IsApproved == false)
        .Select(n => new PhotoForApprovalDTO
        {
          Id = n.Id,
          Username = n.User.UserName,
          Url = n.Url,
          IsApproved = n.IsApproved
        }).ToListAsync();

      return  photos;

    }

    public void RemovePhoto(Photo picture)
    {
     _context.Photos.Remove(picture);
    }
  }
}
