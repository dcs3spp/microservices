using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using dcs3spp.courseManagementContainers.Services.Courses.StoredProcedures;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;

namespace Courses.API.Infrastructure.Migrations
{
    [DbContext(typeof(CourseContext))]
    [Migration("CustomMigration_StoredProcedures")]
    public partial class StoredProcedureMigrations : Migration
    {   
        private const string pattern = @"^(?:\w+\.)+(?:(?<create>Postgresql\.Create(?:\.\w+)+)|(?<drop>Postgresql\.Drop(?:\.\w+)+))\n?$";
        
        private Assembly Assembly { get; set; }
        private EmbeddedResourceParser Parser { get; set; }

        public StoredProcedureMigrations() 
        {
            Assembly = typeof(EmbeddedResourceParser).Assembly;
            Parser = new EmbeddedResourceParser(Assembly, pattern);
            Parser.Parse();
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ExecuteScripts(migrationBuilder, Parser.CreateScripts);
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ExecuteScripts(migrationBuilder, Parser.DropScripts);
        }

        private void ExecuteScripts(MigrationBuilder builder, IReadOnlyCollection<Match> scripts)
        {
            foreach(Match match in scripts)
            {
                using (Stream stream = Assembly.GetManifestResourceStream(match.Value))
                using (StreamReader reader = new StreamReader(stream))
                {
                    builder.Sql(reader.ReadToEnd());
                }
            }
        }
    }
}
