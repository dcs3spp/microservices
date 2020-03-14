namespace Courses.FunctionalTests
{
    public static class ApiRoutes 
    {
        public static class Get
        {
            public static string Courses = "api/v1/courses";

            public static string Course(int id)
            {
                return $"api/v1/courses/{id}";
            }

            public static string CourseUnits(int courseId)
            {
                return $"api/v1/courses/{courseId}/units";
            }
        }

        public static class Post
        {
            public static string CreateCourse = "api/v1/courses";
        }
    }
}