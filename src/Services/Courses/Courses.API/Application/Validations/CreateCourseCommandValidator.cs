using FluentValidation;
using dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands;
using Microsoft.Extensions.Logging;
// using static dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands.CreateCourseCommand;

namespace Courses.API.Application.Validations
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator(ILogger<CreateCourseCommandValidator> logger)
        {
            RuleFor(command => command.CourseId).NotEmpty().WithMessage("CourseId is missing");
            RuleFor(command => command.CourseUnits).NotNull().WithMessage("List of course units missing");
            RuleFor(command => command.Description).NotEmpty().WithMessage("Course description is missing");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}