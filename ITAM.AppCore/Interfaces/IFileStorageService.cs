using System.IO;

namespace ITAM.AppCore.Interfaces
{
    public interface IFileStorageService
    {
        string GetFullPath(string relativePath);

        Task<string> SaveAsync(
            Stream stream,
            string folder,
            string extension);

        Task DeleteAsync(string? relativePath);
    }
}