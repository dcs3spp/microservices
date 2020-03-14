using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

using dcs3spp.courseManagementContainers.Services.Courses.Courses.DTO;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;
using dcs3spp.courseManagementContainers.Services.Courses.API;
using dcs3spp.courseManagementContainers.Services.Courses.API.Infrastructure;
using dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF;
using dcs3spp.Testing.Integration;


namespace Courses.FunctionalTests
{
    [Collection("FunctionalDbTests")]
    public class CoursesScenarios
    {
        private TestingBase<CoursesTestStartup> _testing;
        
        public CoursesScenarios(ITestOutputHelper output)
        { 
            _testing = new TestingBase<CoursesTestStartup>(output);
            _testing.Factory.OnServerCreated = new Action<TestServer>((server)=> {
                TestSeed t = new TestSeed(server);
                t.SeedAsync().Wait();
            });
        }

        [Fact]
        public async Task Get_Courses()
        {
            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.Courses);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            
            const string expectedJson = @"[{""courseid"":1014760,""description"":""BTEC L3 ICT""},{""courseid"":1013760,""description"":""BTEC L3 90 ICT""},{""courseid"":1015423,""description"":""BTEC L2 ICT""}]";
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task Get_Course()
        {
            const int courseID = 1014760;
           
            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.Course(courseID));
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            
            const string expectedJson = @"{""courseid"":1014760,""description"":""BTEC L3 ICT""}";
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task Get_Course_Not_Found()
        {
            const int courseID = 1014769;
            
            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.Course(courseID));
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_Course_Units()
        {
            const int courseID = 1014760;
            
            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.CourseUnits(courseID));
            string result = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            const string expectedJson = @"[{""unitid"":1,""description"":""Computer Systems""},{""unitid"":2,""description"":""Advanced Spreadsheet Modelling""}]";
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task Get_Course_Units_Empty()
        {
            const int courseID = 1013760;

            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.CourseUnits(courseID));
            string result = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            const string expectedJson = @"[]";
            AssemblyLoadEventArgs.Equals(expectedJson, result);
        }

        [Fact]
        public async Task Get_Course_Units_Course_Not_Found()
        {
            const int courseID = 2014760;

            var response = await _testing.Factory.CreateClient().GetAsync(ApiRoutes.Get.CourseUnits(courseID));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_Course()
        {
            const string expectedLoc = @"http://localhost/api/v1/Courses/1014763";
            const string expectedReason = @"Created";
            const string expectedResponse = @"{""courseID"":1014763,""description"":""Course Test"",""units"":[]}";

            var stringContent = new StringContent(BuildCourse(1014763,"Course Test", new List<UnitDTO>()), Encoding.UTF8, "application/json");
            var response = await _testing.Factory.CreateClient().PostAsync(ApiRoutes.Post.CreateCourse, stringContent);

            string result = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(expectedResponse, result);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(expectedLoc, response.Headers.Location.ToString());
            Assert.Equal(expectedReason, response.ReasonPhrase);
        }

        [Fact]
        public async Task Post_Course_Missing_ID()
        {
            const string expectedResult=@"{""errors"":{""DomainValidations"":[""CourseId is missing""]},""title"":""One or more validation errors occurred."",""status"":400,""detail"":""Please refer to the errors property for additional details."",""instance"":""/api/v1/courses""}";

            var stringContent = new StringContent(BuildCourseMissingID(), Encoding.UTF8, "application/json");
            var response = await _testing.Factory.CreateClient().PostAsync(ApiRoutes.Post.CreateCourse, stringContent);

            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedResult,result);
        }

        [Fact]
        public async Task Post_Course_Missing_Description()
        {
            const string expectedResult=@"{""errors"":{""DomainValidations"":[""Course description is missing""]},""title"":""One or more validation errors occurred."",""status"":400,""detail"":""Please refer to the errors property for additional details."",""instance"":""/api/v1/courses""}";

            var stringContent = new StringContent(BuildCourseMissingDescription(), Encoding.UTF8, "application/json");
            var response = await _testing.Factory.CreateClient().PostAsync(ApiRoutes.Post.CreateCourse, stringContent);

            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedResult,result);
        }

        [Fact]
        public async Task Post_Course_Duplicate()
        {
            const int courseId = 1014760;
            const string courseDescription = "Duplicate Course";
            const int expectedErrorCode = (int) HttpStatusCode.Conflict;
            string expectedErrorMessage = $"Course {courseId} already exists";

            var stringContent = new StringContent(BuildCourse(courseId, courseDescription, new List<UnitDTO>()), Encoding.UTF8, "application/json");
            var response = await _testing.Factory.CreateClient().PostAsync(ApiRoutes.Post.CreateCourse, stringContent);

            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedErrorCode,(int)response.StatusCode);
            Assert.Equal(expectedErrorMessage,result);
        }


        /// <summary>
        /// Create a JSON representation of a Course
        /// {"courseID":1014763,"description":"Course Test","units":[]}
        /// </summary>
        private string BuildCourse(int courseId, string description, List<UnitDTO> units)
        {
            CourseDTO course = new CourseDTO();
            course.CourseId = courseId;
            course.Description = description;
            course.CourseUnits = units;

            string obj = JsonConvert.SerializeObject(course);
            return obj;
        }

        private string BuildCourseMissingID()
        {
            CourseDTO course = new CourseDTO();
            course.Description = "Course Test";
            course.CourseUnits = new List<UnitDTO>();

            string obj = JsonConvert.SerializeObject(course);
            return obj;
        }

        private string BuildCourseMissingDescription()
        {
            CourseDTO course = new CourseDTO();
            course.CourseId = 1015762;
            course.CourseUnits = new List<UnitDTO>();

            string obj = JsonConvert.SerializeObject(course);
            return obj;
        }
    }
}
