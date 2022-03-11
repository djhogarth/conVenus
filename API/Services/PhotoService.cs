using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
  public class PhotoService : IPhotoService
  {
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
      var acc = new Account
      (
        config.Value.CloudName,
        config.Value.ApiKey,
        config.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
      //used to store reponse from cloudinary
      var uploadResult = new ImageUploadResult();
      //upload file to cloudinary if file contains any contents
      if(file.Length > 0){
        //make file a stream of data and create upload parameters
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
          //crop image to a sqaure and focus on the face
          File = new FileDescription(file.FileName, stream),
          Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
        };
        //upload file to cloudinary
        uploadResult = await _cloudinary.UploadAsync(uploadParams);
      }
      return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
      //create deletion parameters 
     var deleteParams = new DeletionParams(publicId);

     var result = await _cloudinary.DestroyAsync(deleteParams);

     return result;
    }
  }
}
