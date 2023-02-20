using System;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.AspNetCore.StaticFiles;
using static System.Net.Mime.MediaTypeNames;

namespace HousePlantMeasurementsApi.Services.ImageService
{
    public class ImageService: IImageService
    {
        private readonly IWebHostEnvironment environment;
        public readonly string[] allowedExtensions;
        public readonly string relativePath;

        public ImageService(IWebHostEnvironment environment)
        {
            this.environment = environment;
            this.allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
            this.relativePath = "/Resources/PlantImages/";
        }

        public async Task<string> SaveImageToFileSystem(IFormFile image)
        {
            if (image.Length > 0 && allowedExtensions.Contains(Path.GetExtension(image.FileName)))
            {
                string directoryPath = environment.ContentRootPath + relativePath;
                string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fffffff") + Path.GetExtension(image.FileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (FileStream fs = System.IO.File.Create(directoryPath + fileName))
                {
                    image.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }

                return fileName;
            }
            else
            {
                throw new FormatException();
            }
        }

        public async Task<bool> RemoveImageFromFileSystem(string imageName)
        {
            string directoryPath = environment.ContentRootPath + relativePath;
            string fullPath = directoryPath + imageName;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return false;
            }
            try
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<byte[]?> GetImageFromPath(string imageName)
        {
            string fullImagePath = environment.ContentRootPath + relativePath + imageName;

            if (!System.IO.File.Exists(fullImagePath))
            {
                return null;
            }

            byte[] rawImage = System.IO.File.ReadAllBytes(fullImagePath);

            return rawImage;
        }

        public async Task<string?> GetImageContentType(string imageName)
        {
            string fullImagePath = environment.ContentRootPath + relativePath + imageName;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(fullImagePath, out contentType);

            return contentType;
        }
    }
}

