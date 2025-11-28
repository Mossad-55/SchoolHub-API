using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Exceptions;

namespace SchoolHubAPI.FilesHandling;

public class FileService : IFileService
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public FileService()
    {
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public void DeleteFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var normalizedPath = path.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedPath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (IsValidImage(file))
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(_uploadPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"uploads/{fileName}";
        }

        return string.Empty;
    }

    private bool IsValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        const long maxSize = 10 * 1024 * 1024;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new FileExtensionBadRequest();

        if (file.Length > maxSize)
            throw new FileSizeBadRequest();

        return true;
    }
}