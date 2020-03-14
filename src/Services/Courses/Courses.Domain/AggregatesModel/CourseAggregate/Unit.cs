using System.Collections.Generic;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.Seedwork;

namespace dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate
{
    public class Unit : Entity { 

         /** 
         * Expose read-only access to collections and explicitly provide methods for
         * clients to interact. Preserves valid state and imutability. 
         */
        private readonly List<CourseUnit> _courses;
        public IReadOnlyCollection<CourseUnit> CourseUnits => _courses;

        public short Code { get; private set; }      
        public string Description { get; private set; }

        protected Unit() {
            _courses = new List<CourseUnit>();
        }

        public Unit(short code, string description) : this() {
            Code = code;
            Description = description;
        }

        public void AddCourse(int courseId) {
            
        } 
    }
}