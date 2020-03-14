using MediatR;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Courses.API.Application.Models;
using System.Linq;
using dcs3spp.courseManagementContainers.BuildingBlocks.Validation;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;

/**
 * A command is a special type of DTO that changes the underlying state
 * It is recommended that it is immutable
 * It references models and only has properties that it needs to enable the update
 */
namespace dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands {

    // DDD and CQRS patterns comment: Note that it is recommended to implement immutable Commands
    // In this case, its immutability is achieved by having all the setters as private
    // plus only being able to update the data just once, when creating the object through its constructor.
    // References on Immutable Commands:  
    // http://cqrs.nu/Faq
    // https://docs.spine3.org/motivation/immutability.html 
    // http://blog.gauffin.org/2012/06/griffin-container-introducing-command-support/
    // https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/how-to-implement-a-lightweight-class-with-auto-implemented-properties

    [DataContract]
    public class CreateCourseCommand
        : IRequest<CommandResult<CourseDTO>>
    {
        [DataMember]
        private readonly List<UnitDTO> _courseUnits;

        [DataMember]
        public int CourseId { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public IEnumerable<UnitDTO> CourseUnits => _courseUnits;

        public CreateCourseCommand()
        {
            _courseUnits = new List<UnitDTO>();
        }

        public CreateCourseCommand(List<CourseUnitModel> courseUnits, int courseId, string description) : this()
        {
            _courseUnits = courseUnits.ToCourseUnitsDTO().ToList();
            CourseId = courseId;
            Description = description;
        }
        
        public class UnitDTO
        {
            public short Code { get; set; }

            public string Description { get; set; }
        }
    }
    
    public class CourseDTO
    {
        public int CourseID { get; set; }

        public string Description { get; set; }

        public List<CreateCourseCommand.UnitDTO> Units { get; set; }

        public static CourseDTO FromCourse(Course course)
        {
            CourseDTO dto = new CourseDTO();
            
            dto.CourseID = course.Id;
            dto.Description = course.Description;
            dto.Units = course.CourseUnits.Select(oi => new CreateCourseCommand.UnitDTO
                {
                    Code = oi.Unit.Code,
                    Description = oi.Unit.Description
                }).ToList();

            return dto;
        }
    }
}
