using System.Xml.Linq;

namespace SchoolHubAPI.Entities.Exceptions;

public sealed class CourseExistsException : BadRequestException
{
    public CourseExistsException(string code) 
        : base($"Course with this code: {code} does exist for a department. Please use a different code.")
    {
    }
}
