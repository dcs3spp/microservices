using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Reflection;

namespace dcs3spp.courseManagementContainers.Services.Courses.StoredProcedures
{
    class CreateStoredProcedureOperation : MigrationOperation
    {
        public Assembly Assembly { get; set; }
        public string RegularExpression { get; set; }
    }
}