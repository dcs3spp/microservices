using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using dcs3spp.courseManagementContainers.BuildingBlocks.Validation;
using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Extensions;
using dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands;
using dcs3spp.courseManagementContainers.Services.Courses.API.Application.Queries;
using dcs3spp.courseManagementContainers.Services.Courses.API.Infrastructure.Services;

namespace dcs3spp.courseManagementContainers.Services.Courses.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICourseQueries _courseQueries;
        private readonly IIdentityService _identityService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            IMediator mediator,
            ICourseQueries courseQueries,
            IIdentityService identityService,
            ILogger<CoursesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _courseQueries = courseQueries ?? throw new ArgumentNullException(nameof(courseQueries));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{courseId:int}", Name="GetCourse")]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCourseAsync(int courseId)
        {
            try
            {
                var course = await _courseQueries.GetCourseAsync(courseId);

                return Ok(course);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet(Name="GetCourses")]
        [ProducesResponseType(typeof(IEnumerable<Course>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesAsync()
        {
            var courses = await _courseQueries.GetCoursesAsync();

            return Ok(courses);
        }

        [HttpGet("{courseId:int}/units", Name="GetCourseUnits")]
        [ProducesResponseType(typeof(IEnumerable<CourseUnit>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CourseUnit>>> GetCourseUnitsAsync(int courseId)
        {
            try
            {
                var courseUnits = await _courseQueries.GetCourseUnitsAsync(courseId);
            
                return Ok(courseUnits);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseCommand createCourseCommand)
        {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createCourseCommand.GetGenericTypeName(),
                nameof(createCourseCommand.CourseId),
                createCourseCommand.CourseId,
                createCourseCommand);

            CommandResult<CourseDTO> result = await _mediator.Send(createCourseCommand);
            if(result.Success)
            {
                return CreatedAtRoute(
                    routeName: "GetCourse",
                    routeValues: new { courseId = createCourseCommand.CourseId },
                    value: result.Result);
            }
          
            switch(result.Error.HTTPStatusCode)
            {
                case (int) HttpStatusCode.Conflict:
                    return Conflict(result.Error.Message);
                default:
                    return new ObjectResult(new { StatusCode = HttpStatusCode.InternalServerError, Value = "Internal Server Error"});
            }
        }
    }
}