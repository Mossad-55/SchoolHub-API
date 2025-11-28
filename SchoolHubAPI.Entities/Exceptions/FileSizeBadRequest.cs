namespace SchoolHubAPI.Entities.Exceptions;

public sealed class FileSizeBadRequest : BadRequestException
{
    public FileSizeBadRequest() 
        : base("File size can't exceed 10 MB.")
    {
    }
}
