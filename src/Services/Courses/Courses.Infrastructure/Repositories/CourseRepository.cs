using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Courses.Domain.Exceptions;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.Seedwork;

namespace dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.Repositories
{
    public class CourseRepository
        : ICourseRepository
    {
        private readonly CourseContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CourseRepository(CourseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Add a course to the repository
        /// </summary>
        /// <param name="course">Course domain object</param>
        /// <exception cref="CourseDomainException">Thrown when duplicate course found</exception>
        public async Task<Course> Add(Course course)
        {
            if(await CourseExists(course.Id))
                throw new CoursesDomainException($"Course {course.Id} already exists");
            
            return  _context.Courses.Add(course).Entity;
        }

        /// <summary>
        /// Add a unit to the repository
        /// </summary>
        /// <param name="unit">Course unit domain object</param>
        /// <remarks>A unit can exist with the same unit code on a different course, e.g.
        /// Unit 2 Computer Systems
        /// Unit 2 Accounting
        ///  </remarks>
        public Unit Add(Unit unit)
        {
            return _context.Units.Add(unit).Entity;
        }

        /// <summary>
        /// Find a course with a given id
        /// </summary>
        /// <param name="courseId">Course identifier</param>
        /// <exception cref="CourseDomainException">Thrown when no matching course could be found</exception>
        public async Task<Course> FindCourseAsync(int courseId)
        {
             var course = await _context.Courses
                .Include(c => c.CourseUnits)
                .ThenInclude(cu => cu.Unit)
                .Where(c => c.Id == courseId)
                .SingleOrDefaultAsync();

            if(course == null)
                throw new CoursesDomainException($"Course {courseId} not found");
            return course;
        }

        /// <summary>
        /// Find a unit with a given id
        /// </summary>
        /// <param name="unitId">Unit identifier</param>
        /// <exception cref="CourseDomainException">Thrown when no matching unit could be found</exception>
        public async Task<Unit> FindUnitAsync(int unitId)
        {
            var unit = await _context.Units
                .Where(u => u.Id == unitId)
                .SingleOrDefaultAsync();

            if(unit == null)
                throw new CoursesDomainException($"Unit {unitId} not found");
            return unit;
        }

        /// <summary>
        /// Register an existing unit for a course
        /// </summary>
        /// <param name="unitId">Unit identifier</param>
        /// <param name="course">Course entity</param>
        /// <exception cref="Courses.Domain.Exceptions.CourseDomainException">
        /// Thrown when unit does not exist or a unit with same code exists
        /// </exception>        
        public async Task RegisterUnit(Course course, int unitId)
        {
            // retrieve the unit
            Unit unit = await _context.Units
                .Where(u => u.Id == unitId)
                .SingleOrDefaultAsync();

            if(unit  == null)
                throw new CoursesDomainException($"Unit {unitId} does not exist");
            
            if(course.HasUnitWithCode(unit.Code)) 
                throw new CoursesDomainException($"Unit with code {unit.Code} already registered for course");
            else
                _context.CourseUnits.Add(new CourseUnit(course.Id,unitId));
        }

        public void Update(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
        }

        public void Delete(Course course)
        {
            _context.Remove(course);
        }

        private async Task<bool> CourseExists(int courseId)
        {
            return await _context.Courses.AnyAsync(course => course.Id == courseId);
        }
    }
}