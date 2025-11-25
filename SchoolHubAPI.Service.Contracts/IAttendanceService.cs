using SchoolHubAPI.Shared.DTOs.Attendance;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IAttendanceService
{
    Task<(IEnumerable<AttendanceDto> AttendanceDtos, MetaData MetaData)> GetAttendanceForBatchAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool attTrackChanges);
    Task<(IEnumerable<AttendanceDto> AttendanceDtos, MetaData MetaData)> GetAttendanceForStudentAsync(Guid batchId, Guid studentId, RequestParameters requestParameters, bool batchTrackChanges, bool attTrackChanges);
    Task CreateAttendanceAsync(Guid batchId, Guid teacherId, AttendanceForCreationDto creationDto, bool batchTrackChanges, bool attTrackChanges);
    Task UpdateAttendanceAsync(Guid batchId, Guid attendanceId, AttendanceForUpdateDto updateDto, bool batchTrackChanges, bool attTrackChanges);
    Task DeleteAttendanceAsync(Guid batchId, Guid attendanceId, bool batchTrackChanges, bool attTrackChanges);
}
