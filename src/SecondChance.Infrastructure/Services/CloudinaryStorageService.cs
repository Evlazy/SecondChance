using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql.BackendMessages;
using SecondChance.Application.Interfaces;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be empty");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folderName,
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }

    public async Task DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        // Extract the public ID from the Cloudinary URL to support deletion
        var uri = new Uri(fileUrl);
        var pathSegments = uri.AbsolutePath.Split('/');

        // Get filename without extension
        var fileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
        var folder = pathSegments[pathSegments.Length - 2];
        var publicId = $"{folder}/{fileName}";

        var deletionParams = new DeletionParams(publicId);
        await _cloudinary.DestroyAsync(deletionParams);
    }
}