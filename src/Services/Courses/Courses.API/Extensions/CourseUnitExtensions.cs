using System.Collections.Generic;

using static dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands.CreateCourseCommand;

namespace Courses.API.Application.Models
{
    public static class CourseUnitExtensions
    {
        public static IEnumerable<UnitDTO> ToCourseUnitsDTO(this IEnumerable<CourseUnitModel> courseUnits)
        {
            foreach (var unit in courseUnits)
            {
                yield return unit.ToCourseUnitDTO();
            }
        }

        public static UnitDTO ToCourseUnitDTO(this CourseUnitModel unit)
        {
            return new UnitDTO()
            {
                Code = unit.Code,
                Description = unit.Description
            };
        }
    }
}
