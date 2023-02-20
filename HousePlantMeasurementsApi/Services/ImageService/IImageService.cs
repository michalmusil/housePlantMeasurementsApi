using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.User;

namespace HousePlantMeasurementsApi.Services.ImageService
{
    public interface IImageService
    { 
        public Task<string> SaveImageToFileSystem(IFormFile image);

        public Task<bool> RemoveImageFromFileSystem(string imageName);

        public Task<byte[]?> GetImageFromPath(string imageName);

        public Task<string?> GetImageContentType(string imageName);
    }
}

