using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace dcs3spp.courseManagementContainers.Services.Courses.StoredProcedures
{
    class MyMigrationsNpsqlGenerator : NpgsqlMigrationsSqlGenerator
    {
        public MyMigrationsNpsqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IMigrationsAnnotationProvider migrationsAnnotations,
            INpgsqlOptions opts
            )
            : base(dependencies, migrationsAnnotations, opts)
        {
        }

        protected override void Generate(
            MigrationOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            if (operation is CreateStoredProcedureOperation createStoredProcedureOperation)
            {
                Generate(createStoredProcedureOperation, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }

        private void Generate(
            CreateStoredProcedureOperation operation,
            MigrationCommandListBuilder builder)
        {
            EmbeddedResourceParser p = new EmbeddedResourceParser(operation.Assembly, operation.RegularExpression);
            p.Parse();

            

            // builder
            //     .Append("CREATE USER ")
            //     .Append(sqlHelper.DelimitIdentifier(operation.Name))
            //     .Append(" WITH PASSWORD = ")
            //     .Append(stringMapping.GenerateSqlLiteral(operation.Password))
            //     .AppendLine(sqlHelper.StatementTerminator)
            //     .EndCommand();
        }
    }
}