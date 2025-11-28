namespace SchoolHubAPI.Entities.Exceptions;

public sealed class FileExtensionBadRequest : BadRequestException
{
    public FileExtensionBadRequest() 
        : base("Only .pdf, .doc, and .docx files are allowed.")
    {
    }
}
