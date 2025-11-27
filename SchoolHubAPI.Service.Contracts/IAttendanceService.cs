using SchoolHubAPI.Shared.DTOs.Attendance;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IAttendanceService
{
    Task<(IEnumerable<AttendanceDto> AttendanceDtos, MetaData MetaData)> GetAttendanceForBatchAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool attTrackChanges);
    Task<AttendanceDto> GetAttendanceForStudentAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool attTrackChanges);
    Task<AttendanceDto> GetAttendanceForBatchAsync(Guid batchId, Guid id, bool batchTrackChanges, bool attTrackChanges);
    Task<AttendanceDto?> CreateAttendanceAsync(Guid batchId, Guid teacherId, AttendanceForCreationDto creationDto, bool batchTrackChanges, bool attTrackChanges);
    Task UpdateAttendanceAsync(Guid batchId, Guid teacherId, Guid id, AttendanceForUpdateDto updateDto, bool batchTrackChanges, bool attTrackChanges);
    Task DeleteAttendanceAsync(Guid batchId, Guid teacherId, Guid id, bool batchTrackChanges, bool attTrackChanges);
}
