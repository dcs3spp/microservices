namespace dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate
{
    /**
     * Class that represents a join table for course and unit entities
     * Used in the CourseContext class for creating table in underlying database when
     * migrations applied. N.B no base class of Entity or Aggregate root
     */
    public class CourseUnit {
        public int CourseId { get; private set; }
        public int UnitId { get; private set; }
        public Unit Unit { get; private set; }

        protected CourseUnit() {

        }
        public CourseUnit(int courseId, short unitCode, string unitTitle) : this () {
            CourseId = courseId;
            
            Unit = new Unit(unitCode, unitTitle);
        }

        public CourseUnit(int courseId, int unitId) : this () {
            CourseId = courseId;
            UnitId = unitId;
        }
    }
}
