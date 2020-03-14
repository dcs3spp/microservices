using Courses.Domain.Exceptions;

namespace dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands
{
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
   
    using dcs3spp.courseManagementContainers.BuildingBlocks.Validation;
    using dcs3spp.courseManagementContainers.Services.Courses.API.Infrastructure.Services;
    using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.Idempotency;
    using Domain.AggregatesModel.CourseAggregate;
    using global::Courses.API.Application.IntegrationEvents;

    
    // Regular CommandHandler
    public class CreateCourseCommandHandler
        : IRequestHandler<CreateCourseCommand, CommandResult<CourseDTO>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly ICoursesIntegrationEventService _courseIntegrationEventService;
        private readonly ILogger<CreateCourseCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public CreateCourseCommandHandler(IMediator mediator,
            ICoursesIntegrationEventService courseIntegrationEventService,
            ICourseRepository orderRepository,
            IIdentityService identityService,
            ILogger<CreateCourseCommandHandler> logger)
        {
            _courseRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _courseIntegrationEventService = courseIntegrationEventService ?? throw new ArgumentNullException(nameof(courseIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommandResult<CourseDTO>> Handle(CreateCourseCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- CreateCourseCommandHandler");
            
            CommandResult<CourseDTO> result = new CommandResult<CourseDTO>();

            // Add Integration events here if needed to notify other microservices
          
            // Add/Update the Course AggregateRoot in preparation for saving
            Course course = null;
            try 
            {
                course = CreateCourse(message.CourseId, message.Description, message.CourseUnits); 
            }
            catch(CoursesDomainException ex)
            {
                _logger.LogError(ex, "----- Exception encountered by CreateCourseCommandHandler registering units with course");
                result.SetError(ex.Message, StatusCodes.Status409Conflict);
                return result;
            }

            try
            {
                _logger.LogInformation("----- Creating Course - Course: {@Course}", course);
                await SaveCourse(course, cancellationToken);
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex, "----- Exception encountered by CreateCourseCommandHandler saving course to respository");
                 result.SetError(ex.Message, StatusCodes.Status409Conflict);
                 return result;
            }

            CourseDTO dto = CourseDTO.FromCourse(course);
            result.SetResult(dto);
            
            return result;
        }


        private Course CreateCourse(int courseId, string description, IEnumerable<CreateCourseCommand.UnitDTO> units)
        {
            Course course = new Course(courseId, description);
            foreach (var unit in units)
            {
                course.AddUnit(unit.Code, unit.Description);
            } 
            return course;
        }

        private async Task SaveCourse(Course course, CancellationToken cancellationToken)
        {
            await _courseRepository.Add(course);
            await _courseRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
}


    // Use for Idempotency in Command process
    public class CreateCourseIdentifiedCommandHandler : IdentifiedCommandHandler<CreateCourseCommand, CommandResult<CourseDTO>>
    {
        public CreateCourseIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateCourseCommand, CommandResult<CourseDTO>>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override CommandResult<CourseDTO> CreateResultForDuplicateRequest()
        {
            
            return new CommandResult<CourseDTO>(); // Ignore duplicate requests for creating course.
        }
    }
}