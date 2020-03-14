using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

using GrpcCourses;
using dcs3spp.Testing.Integration;

using Microsoft.AspNetCore.TestHost;

namespace Courses.FunctionalTests
{
    [Collection("FunctionalDbTests")]
    public class CoursesGrpcScenarios
    {
        private const string BaseAddress = "http://localhost";
        private TestingBase<CoursesTestStartup> _testing;

        public CoursesGrpcScenarios(ITestOutputHelper output)
        { 
            _testing = new TestingBase<CoursesTestStartup>(output);
            _testing.Factory.OnServerCreated = new Action<TestServer>((server)=> {
                TestSeed t = new TestSeed(server);
                t.SeedAsync().Wait();
            });
            
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
        }

        [Fact]
        public async Task Create_Course()
        {
            using(GrpcChannel channel = _testing.Factory.CreateChannel(BaseAddress))
            {
                CoursesGrpc.CoursesGrpcClient client = new CoursesGrpc.CoursesGrpcClient(channel);

                CreateCourseCommand com= new CreateCourseCommand();
                com.CourseId = 123456;
                com.CourseName = "TestCourse";
                    
                CourseDTO dto = await client.CreateCourseAsync(com);

                Assert.Equal(com.CourseId, dto.CourseId);
                Assert.Equal(com.CourseName, dto.CourseName);
            }
        }

        [Fact]
        public async void Create_Course_Missing_Id()
        {
            using(GrpcChannel channel = _testing.Factory.CreateChannel(BaseAddress))
            {
                CoursesGrpc.CoursesGrpcClient client = new CoursesGrpc.CoursesGrpcClient(channel);

                CreateCourseCommand com= new CreateCourseCommand();
                com.CourseName = "TestCourse";
                    
                await Assert.ThrowsAsync<Grpc.Core.RpcException>(async () => await client.CreateCourseAsync(com));
            }
        }

        [Fact]
        public async void Create_Course_Missing_Description()
        {
            using(GrpcChannel channel = _testing.Factory.CreateChannel(BaseAddress))
            {
                CoursesGrpc.CoursesGrpcClient client = new CoursesGrpc.CoursesGrpcClient(channel);

                CreateCourseCommand com= new CreateCourseCommand();
                com.CourseId = 10234;
                    
                await Assert.ThrowsAsync<Grpc.Core.RpcException>(async () => await client.CreateCourseAsync(com));
            }
        }

        [Fact]
        public async void Create_Course_Duplicate()
        {
            using(GrpcChannel channel = _testing.Factory.CreateChannel(BaseAddress))
            {
                const int courseId = 1014760;
                string expectedErrorMessage = $"Course {courseId} already exists";
                
                CoursesGrpc.CoursesGrpcClient client = new CoursesGrpc.CoursesGrpcClient(channel);

                CreateCourseCommand com= new CreateCourseCommand();
                com.CourseId = 1014760;
                com.CourseName = "Duplicate Course";
                
                Grpc.Core.RpcException ex = await Assert.ThrowsAsync<Grpc.Core.RpcException>(async () => await client.CreateCourseAsync(com));
                Assert.Equal(Grpc.Core.StatusCode.AlreadyExists, ex.StatusCode);
                Assert.Equal(expectedErrorMessage, ex.Status.Detail);
                
            } 
        }
    }
}
