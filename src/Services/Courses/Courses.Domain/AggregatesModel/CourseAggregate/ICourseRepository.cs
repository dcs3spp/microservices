using dcs3spp.courseManagementContainers.Services.Courses.Domain.Seedwork;
using System.Threading.Tasks;

namespace dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate
{
    // This is just the RepositoryContracts or Interface defined at the Domain Layer
    // as requisite for the Courses Aggregate
    // This is an example of Separated Interface pattern recommended by Martin Fowler.
    // Can use dependency injection to isolate implementation and delegate to 
    // infrastructure / persistence layer, e.g. EF Core. 
    // This way the presentation layer does not depend upon the infrastructure (EF Core) layer.
    //
    // Repository pattern increases testability, can mock fake data for example.
    // However, not critical to DDD design.

    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> Add(Course course);

        Unit Add(Unit unit);
        
        void Delete(Course course);

        void Update(Course course);

        /// Retrieve course by id
        /// <summary>
        /// Retrieve course for a given id
        /// </summary>
        Task<Course> FindCourseAsync(int courseId);
        
        /// Retrieve unit by id
        /// <summary>
        /// Retrieve unit by id
        Task<Unit> FindUnitAsync(int unitId);

        Task RegisterUnit(Course course, int unitid);
    }
}
