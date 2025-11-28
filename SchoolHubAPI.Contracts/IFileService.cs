using Microsoft.AspNetCore.Http;

namespace SchoolHubAPI.Contracts;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
    void DeleteFile(string path);
}
