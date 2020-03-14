using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppCommand = dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands;
using ApiModels = Courses.API.Application.Models;
using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Extensions;
using Google.Protobuf.Collections;

namespace GrpcCourses
{
    /// <remarks>
    /// Grpc is ideal for use in fast synchronous communication over traditional HTTP JSON since uses binary serialisation
    /// and supports streaming.
    ///
    /// For example it can be used between:
    ///  -  Aggregator and other microservices
    ///  -  BFF for mobile devices and microservices
    /// </remarks>
    public class CoursesService : CoursesGrpc.CoursesGrpcBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CoursesService> _logger;

        public CoursesService(IMediator mediator, ILogger<CoursesService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<CourseDTO> CreateCourse(CreateCourseCommand createCourseCommand, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for courses create course {CreateCourseCommand}", context.Method, createCourseCommand);
            _logger.LogTrace(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createCourseCommand.GetGenericTypeName(),
                nameof(createCourseCommand.CourseId),
                createCourseCommand.CourseId,
                createCourseCommand);

            var command = new AppCommand.CreateCourseCommand(this.MapUnits(createCourseCommand.Units).ToList(), createCourseCommand.CourseId,
                createCourseCommand.CourseName);

            var data = await _mediator.Send(command);

            if (data != null && data.Success)
            {
                context.Status = new Status(StatusCode.OK, $"courses create course {createCourseCommand} created");

                return this.MapResponse(data.Result);
            }
            else if(data != null)
            {
                context.Status = new Status(data.Error.MapToGrpStatusCode(), $"{data.Error.Message}");
            }

            return new CourseDTO();
        }

        public CourseDTO MapResponse(AppCommand.CourseDTO course)
        {
            var result = new CourseDTO()
            {
                CourseId = course.CourseID,
                CourseName = course.Description
            };

            course.Units.ToList().ForEach(i => result.Units.Add(new CourseUnitDTO()
            {
                UnitId = i.Code,
                UnitName = i.Description,
            }));

            return result;
        }

        public IEnumerable<ApiModels.CourseUnitModel> MapUnits(RepeatedField<CourseUnit> units)
        {
            return units.Select(x => new ApiModels.CourseUnitModel()
            {
                Code = (short) x.UnitCode,
                Description = x.UnitName
            });
        }
    }
}
