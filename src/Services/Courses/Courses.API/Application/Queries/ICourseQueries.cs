namespace dcs3spp.courseManagementContainers.Services.Courses.API.Application.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICourseQueries
    {
        Task<Course> GetCourseAsync(int courseId);

        Task<IEnumerable<Course>> GetCoursesAsync();

        Task<IEnumerable<CourseUnit>> GetCourseUnitsAsync(int courseId);
    }
}
