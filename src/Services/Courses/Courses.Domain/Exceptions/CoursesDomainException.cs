using System;

namespace Courses.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class CoursesDomainException : Exception
    {
        public CoursesDomainException()
        { }

        public CoursesDomainException(string message)
            : base(message)
        { }

        public CoursesDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}