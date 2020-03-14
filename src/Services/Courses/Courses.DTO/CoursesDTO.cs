using System;
using System.Collections.Generic;

namespace dcs3spp.courseManagementContainers.Services.Courses.Courses.DTO
{
    public class CourseDTO
    {
        public int CourseId { get; set; }

        public string Description { get; set; }

        public List<UnitDTO> CourseUnits { get; set; }
    }
    public class LearningOutcomeDTO 
    {
        public short Id { get; set; }
        public int Description { get; set; }
        
    }

    public class UnitCriteriaDTO
    {
        public char Criteria { get; set; }
        public string Description { get; set; }
    }

    public class UnitDTO
    {
        
        public short Code { get; set; }
        public string Description { get; set; }
        // public int Id { get; set; }

        // public List<LearningOutcomeDTO> LearningOutcomes { get; set; }
        // public List<UnitCriteriaDTO> UnitCriteria { get; set; }
    }
}
