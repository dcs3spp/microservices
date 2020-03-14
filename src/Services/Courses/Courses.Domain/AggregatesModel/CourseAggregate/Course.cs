using System.Collections.Generic;
using System.Linq;

using Courses.Domain.Exceptions;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.Seedwork;

namespace dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate
{
    /**
     * Domain entities implemented as POCO classes, with no dependence on external libraries
     * Domain entities are mapped to EF Core in infrastructure layer
     *
     * Perform validations in constructors or methods that update the entity.
     * Be aware of different patterns rather than just raising an exception.
     * Could have Specification and Notification pattern, e.g. returning a list
     * of all errors.
     * Also consider performing field level validation on Data Transfer Object (DTO)
     * and domain-level validation in domain entities. Avoids duplicity.
     * Perform validation client side and server side on (DTO).
     *
     * An entity can raise domain events for other entities within the same domain.
     * The event handlers should be in the application layer. An event should be an 
     * immutable class that describes what happened.
     * Use domain events to explictly implement side effects across one or multiple 
     * aggregates. 
     * Use integration events to handle events across Boundary Contexts.
     *
     * In the infrastructure layer have one repository per aggregate root
     */

    /// <summary>
    /// Aggregate root for courses.
    /// </summary>
    /// <remarks>
    /// A course is described by a numerical Id property and a description.
    /// A course can have many units of study registered. Each unit is described by a
    /// short numerical code and description.
    /// It should not be possible to register many units that have the same code on a given course
    /// The Id property inherited from <see cref="Entity"/> denotes the id for a course.
    /// </remarks>
    public class Course : Entity, IAggregateRoot {

        /** 
         * Expose read-only access to collections and explicitly provide methods for
         * clients to interact. Preserves valid state and imutability. 
         */
        private readonly List<CourseUnit> _courseUnits;
        public IReadOnlyCollection<CourseUnit> CourseUnits => _courseUnits;


        public string Description { get; private set; }

        /// <summary>
        /// Default constructor creates empty list of course units
        /// </summary>
        protected Course() {
            _courseUnits = new List<CourseUnit>();
        }

        /// <summary>
        /// Constructor that initialises course Id and description
        /// </summary>
        public Course(int courseId, string description) : this () {
            Id = courseId;
            Description = description;
        }


        /// <summary>
        /// Add a Unit
        /// </summary>
        /// <param name="title">Unit title</param>
        /// <param name="code">Unit code</param>
        /// <remarks>
        /// Throws <cref="CoursesDomainException"/> if a unit is already registered on the course with that code
        /// </remarks>
        public void AddUnit(short code, string title) {
            
            if(HasUnitWithCode(code)) {
                ExistingUnitException(code);
            }
            else {
                CourseUnit cu = new CourseUnit(Id, code, title);
                _courseUnits.Add(cu);
            }
        }

        /// <summary>
        /// Determine if there is a unit with a given code that is already registered on the course
        /// </summary>
        public bool HasUnitWithCode(short code) {
            
            CourseUnit existingUnit = _courseUnits.Where(cu => cu.Unit.Code == code)
                .SingleOrDefault();

            return (existingUnit != null);
        }

        private void ExistingUnitException(short code) {
            throw new CoursesDomainException($"Unit {code} is already assigned to Course {Description}");
        } 
    }
}