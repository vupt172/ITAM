using ITAM.AppCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ITAM.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _serverRoot;

        public FileStorageService(IConfiguration configuration)
        {
            _serverRoot = configuration["Storage:ServerRoot"]
                ?? throw new InvalidOperationException(
                    "Chưa cấu hình Storage:ServerRoot.");
        }

        public string GetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException(
                    "Đường dẫn file không hợp lệ.",
                    nameof(relativePath));

            var normalizedPath = relativePath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            return Path.Combine(_serverRoot, normalizedPath);
        }

        public async Task<string> SaveAsync(
            Stream stream,
            string folder,
            string extension)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            extension = NormalizeExtension(extension);

            var fileName = $"{Guid.NewGuid():N}{extension}";

            var relativePath = Path.Combine(
                folder,
                fileName);

            var fullPath = GetFullPath(relativePath);

            var directory = Path.GetDirectoryName(fullPath);

            if (directory == null)
                throw new InvalidOperationException(
                    "Không xác định được thư mục lưu file.");

            Directory.CreateDirectory(directory);

            await using var fileStream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await stream.CopyToAsync(fileStream);

            return relativePath.Replace('\\', '/');
        }

        public Task DeleteAsync(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.CompletedTask;

            var fullPath = GetFullPath(relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        private static string NormalizeExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException(
                    "File ảnh phải có phần mở rộng.");

            extension = extension.Trim().ToLowerInvariant();

            return extension switch
            {
                ".jpg" => ".jpg",
                ".jpeg" => ".jpg",
                ".png" => ".png",
                ".webp" => ".webp",
                _ => throw new InvalidOperationException(
                    "Chỉ hỗ trợ ảnh JPG, PNG hoặc WEBP.")
            };
        }
    }
}