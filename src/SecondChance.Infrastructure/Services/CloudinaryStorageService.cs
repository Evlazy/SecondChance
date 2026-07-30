using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SecondChance.Application.Interfaces;
using System.Linq;

namespace SecondChance.Infrastructure.Services;

public sealed class CloudinaryStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration config)
    {
        var cloudName = config["Cloudinary:CloudName"];
        var apiKey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];
        if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            throw new InvalidOperationException("Cloudinary configuration is missing.");

        _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
    }

    public async Task<string> UploadAsync(IFormFile file, string folderName)
    {
        if (file is null || file.Length is <= 0 or > 2 * 1024 * 1024 || !AllowedContentTypes.Contains(file.ContentType))
            throw new ArgumentException("Upload a JPEG, PNG, or WebP image no larger than 2 MB.");

        await using var stream = file.OpenReadStream();
        if (!await HasSupportedImageSignatureAsync(stream))
            throw new ArgumentException("The uploaded content is not a valid JPEG, PNG, or WebP image.");

        stream.Position = 0;
        var result = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription("image", stream),
            Folder = folderName,
            UseFilename = false,
            UniqueFilename = true,
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        });

        if (result.Error is not null || result.SecureUrl is null)
            throw new InvalidOperationException("Image upload failed.");

        return result.SecureUrl.ToString();
    }

    // ProductImage currently stores only the delivery URL. Store Cloudinary PublicId in a future migration
    // before enabling destructive deletion, so URLs cannot be parsed into an unintended asset id.
    public Task DeleteAsync(string fileUrl) => Task.CompletedTask;

    private static async Task<bool> HasSupportedImageSignatureAsync(Stream stream)
    {
        var header = new byte[12];
        var count = await stream.ReadAsync(header);
        if (count < 3) return false;

        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return true;

        ReadOnlySpan<byte> pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        if (count >= 8 && header.AsSpan(0, 8).SequenceEqual(pngSignature))
            return true;

        if (count >= 12 && header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8))
            return true;

        return false;
    }
}
