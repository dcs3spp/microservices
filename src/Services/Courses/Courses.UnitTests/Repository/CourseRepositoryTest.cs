using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System;

using Courses.Domain.Exceptions;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.Repositories;


namespace SimpleToDo.Repository.UnitTest
{
    public class CourseRepositoryTests
    {
        [Fact]
        public async void AddEmptyCourse()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            Course course = new Course(courseId, courseDescription);
            
            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            await sut.Repo.Add(course);
            
            int count = await sut.Context.SaveChangesAsync();

            Assert.True(count > 0);
            
            Course myCourse = await sut.Context.FindAsync<Course>(courseId);
            Assert.Equal(courseId, myCourse.Id);
            Assert.Equal(courseDescription, myCourse.Description);
        }

        [Fact]
        public async void AddCourseWithUnit()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            short unitCode = 2;
            string unitDescription = "Computer Systems";
            
            Course course = new Course(courseId, courseDescription);
            course.AddUnit(unitCode, unitDescription);

            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            await sut.Repo.Add(course);
            
            int count = await sut.Context.SaveChangesAsync();

            Assert.True(count > 0);
            
            Course myCourse = sut.Context.Find<Course>(courseId);
            Assert.Equal(courseId, myCourse.Id);
            Assert.Equal(courseDescription, myCourse.Description);
            
            Assert.Equal(1, myCourse.CourseUnits.Count);

            var enumerator = myCourse.CourseUnits.GetEnumerator();
            enumerator.MoveNext();
            
            CourseUnit cu  = enumerator.Current;
            Assert.NotNull(cu.Unit);
            Assert.Equal(unitCode, cu.Unit.Code);
            Assert.Equal(unitDescription, cu.Unit.Description);
            Assert.True(cu.UnitId > -1);
        }

        [Fact]
        public async void AddDuplicateCourse()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            Course course = new Course(courseId, courseDescription);
            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();

            sut.Context.Courses.Add(course);
            await sut.Context.SaveChangesAsync();

            await Assert.ThrowsAsync<CoursesDomainException>(async () => await sut.Repo.Add(course));
        }

        [Fact]
        public async void DeleteCourse()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            short unitCode = 2;
            string unitDescription = "Computer Systems";
            
            // save a course with one unit
            Course course = new Course(courseId, courseDescription);
            course.AddUnit(unitCode, unitDescription);

            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            await sut.Repo.Add(course);
            
            int count = await sut.Context.SaveChangesAsync();

            Assert.Equal(3, count); // 1 course, 1 unit and 1 associated courseunit

            // delete the course
            sut.Repo.Delete(course);
            count = await sut.Context.SaveChangesAsync();
            
            Assert.Equal(2, count); // 1 course, 1 associated course unit

            // the course should not be in the repository
            // the unit should not be in the repository for the course
            Assert.Null(sut.Context.Find<Course>(courseId));
            
            // the unit should not be registered on this course
            // N.B it could be regiistered on other courses still, that is why the delete is not cascaded to unit
            Assert.Equal(0, await sut.Context.CourseUnits.CountAsync());
        }

        [Fact]
        public async void RegisterUnit()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            short unitCode = 2;
            string unitDescription = "Computer Systems";
            
            Course course = new Course(courseId, courseDescription);
            Unit unit = new Unit(unitCode, unitDescription);

            // add a course and a single unit
            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            sut.Context.Courses.Add(course);
            sut.Context.Units.Add(unit);
            int count = await sut.Context.SaveChangesAsync();

            List<Unit> units = await sut.Context.Units.ToListAsync();

            // check that the unit has been added but not registered for the course
            Assert.Equal(0, course.CourseUnits.Count);

            // register the unit for the course
            await sut.Repo.RegisterUnit(course, units[0].Id);
            count = await sut.Repo.UnitOfWork.SaveChangesAsync();

            Assert.Equal(1, course.CourseUnits.Count);
            var enumerator = course.CourseUnits.GetEnumerator();
            enumerator.MoveNext();

            // check that unit is navigatable in CourseUnit and courseid and unitid properties set
            Assert.NotNull(enumerator.Current.Unit);
            Assert.Equal(courseId, enumerator.Current.CourseId);
            Assert.Equal(units[0].Id, enumerator.Current.UnitId);
            Assert.Equal(unitCode, enumerator.Current.Unit.Code);
            Assert.Equal(unitDescription, enumerator.Current.Unit.Description);
        }

        [Fact]
        public async void RegisterUnitWithDuplicateCode()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            short unitCode = 2;
            string unitDescription = "Computer Systems";
            
            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();

            Course course = new Course(courseId, courseDescription);
            course.AddUnit(unitCode, unitDescription);

            // add a course and units with same code
            Unit anotherUnit = new Unit(unitCode, "L2 Computer Systems");
            sut.Context.Courses.Add(course);
            sut.Context.Units.Add(anotherUnit);
            int count = await sut.Context.SaveChangesAsync();
            
            Exception ex = await Assert.ThrowsAsync<CoursesDomainException>(async () => await sut.Repo.RegisterUnit(course, anotherUnit.Id));
        }

        [Fact]
        public async void FindCourseAsync()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            Course course = new Course(courseId, courseDescription);
            
            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            sut.Context.Courses.Add(course);
            
            int count = await sut.Context.SaveChangesAsync();

            Assert.True(count > 0);
            
            Course myCourse = await sut.Repo.FindCourseAsync(courseId);
            Assert.Equal(courseId, myCourse.Id);
            Assert.Equal(courseDescription, myCourse.Description);
        }

        [Fact]
        public async void FindUnitAsync()
        {
            string courseDescription = "BTEC L3 ICT";
            int courseId = 1014760;

            short unitCode = 2;
            string unitDescription = "Computer Systems";

            Course course = new Course(courseId, courseDescription);
            course.AddUnit(unitCode, unitDescription);

            (ICourseRepository Repo, CourseContext Context) sut = CreateSUT();
            sut.Context.Courses.Add(course);
            
            int count = await sut.Context.SaveChangesAsync();
            Assert.True(count > 0);
            
            List<Unit> unitList = await sut.Context.Units.ToListAsync();
            Assert.True(unitList.Count > 0);

            Unit myUnit = await sut.Repo.FindUnitAsync(unitList[0].Id);
            Assert.Equal(unitList[0].Id, myUnit.Id);
            Assert.Equal(unitList[0].Description, myUnit.Description);
        }

        private (ICourseRepository Repo, CourseContext Context) CreateSUT()
        {
            const string DbName = "CoursesDb";

            DbContextOptions<CourseContext> options;
            DbContextOptionsBuilder<CourseContext> builder = new DbContextOptionsBuilder<CourseContext>();
            builder.UseInMemoryDatabase(databaseName: DbName);
            options = builder.Options;
        
            CourseContext courseContext = new CourseContext(options);
            courseContext.Database.EnsureDeleted();
            courseContext.Database.EnsureCreated();
        
            return (Repo: new CourseRepository(courseContext), Context: courseContext);
        }
    }
}
