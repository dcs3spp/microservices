namespace dcs3spp.courseManagementContainers.Services.Courses.API.Application.Queries {
    using Dapper;
    using Npgsql;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class CourseQueries
        : ICourseQueries
    {
        private string _connectionString = string.Empty;

        public CourseQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<Course> GetCourseAsync(int courseId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<Course>(@"SELECT ""CourseID"" as courseid, ""CourseName"" as description
                     FROM coursemanagement.course c
                     WHERE c.""CourseID""=@courseId",
                     new { courseId });
                
                if(result.AsList().Count == 0)
                    throw new KeyNotFoundException();
                
                return result.AsList()[0];
            }
        }
        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<Course>(@"SELECT ""CourseID"" as courseid, ""CourseName"" as description
                     FROM coursemanagement.course");
            }
        }

        public async Task<IEnumerable<CourseUnit>> GetCourseUnitsAsync(int courseId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@course_id", courseId);

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();       
                    var result = await connection.QueryAsync<CourseUnit>(@"coursemanagement.select_course_units", 
                        parameters, 
                        commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch(PostgresException e)
            {
                if(e.SqlState.Equals(PostgresErrorCodes.DataException))
                {
                    throw new KeyNotFoundException(e.MessageText);
                }
                else 
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
    }
}
