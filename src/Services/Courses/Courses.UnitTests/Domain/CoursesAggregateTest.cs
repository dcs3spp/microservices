using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;

using System;
using Xunit;

using Courses.Domain.Exceptions;

namespace Courses.UnitTests
{
    public class CoursesAggregateTest
    {
        public CoursesAggregateTest () {

        }

        [Fact]
        public void CreateCourse()
        {
            int courseId = 1014760;
            string description = "BTEC L3 Course";
            var course = new Course(courseId, description);

            Assert.Equal(course.Id, courseId);
            Assert.Equal(course.Description, description);
        }

        [Fact]
        public void AddUnit()
        {
            short unitCode = 2;
            
            int courseId = 101760;
            string description = "BTEC L3 Course";
            var course = new Course(courseId, description);

            short expectedCount = 1;

            course.AddUnit(unitCode, description);

            Assert.Equal(expectedCount, course.CourseUnits.Count);

            var enumerator = course.CourseUnits.GetEnumerator();
            enumerator.MoveNext();
            
            CourseUnit cu = enumerator.Current;
            
            Assert.NotNull(cu);
            Assert.NotNull(cu.Unit);

            Assert.Equal(courseId, cu.CourseId);
            Assert.Equal(default(int), cu.UnitId);
            Assert.Equal(cu.Unit.Code, unitCode);
            Assert.Equal(cu.Unit.Description, description);
        }

        [Fact]
        public void AddUnit_FailDuplicate()
        {
            short unitCode = 2;

            int courseId = 101760;
            string description = "BTEC L3 Course";
            var course = new Course(courseId, description);

            course.AddUnit(unitCode, description);

            Exception ex = Assert.Throws<CoursesDomainException>(() => course.AddUnit(unitCode, description));
        }

        [Fact]
        public void HasUnit()
        {
            int courseId = 1014760;
            short unitCode = 1;

            string courseTitle = "Sample course";
            string unitTitle = "Sample Unit";
            
            Course course = new Course(courseId, courseTitle);
            course.AddUnit(unitCode, unitTitle);

            Assert.True(course.HasUnitWithCode(unitCode));
        }
    }
}
