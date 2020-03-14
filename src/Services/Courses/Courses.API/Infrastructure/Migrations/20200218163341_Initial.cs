using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Courses.API.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "coursemanagement");

            migrationBuilder.CreateTable(
                name: "course",
                schema: "coursemanagement",
                columns: table => new
                {
                    CourseID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseName = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.CourseID);
                });

            migrationBuilder.CreateTable(
                name: "unit",
                schema: "coursemanagement",
                columns: table => new
                {
                    UnitID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitCode = table.Column<short>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit", x => x.UnitID);
                });

            migrationBuilder.CreateTable(
                name: "courseunit",
                schema: "coursemanagement",
                columns: table => new
                {
                    CourseID = table.Column<int>(nullable: false),
                    UnitID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courseunit", x => new { x.CourseID, x.UnitID });
                    table.ForeignKey(
                        name: "FK_courseunit_course_CourseID",
                        column: x => x.CourseID,
                        principalSchema: "coursemanagement",
                        principalTable: "course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_courseunit_unit_UnitID",
                        column: x => x.UnitID,
                        principalSchema: "coursemanagement",
                        principalTable: "unit",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_courseunit_UnitID",
                schema: "coursemanagement",
                table: "courseunit",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_unit_UnitCode",
                schema: "coursemanagement",
                table: "unit",
                column: "UnitCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "courseunit",
                schema: "coursemanagement");

            migrationBuilder.DropTable(
                name: "course",
                schema: "coursemanagement");

            migrationBuilder.DropTable(
                name: "unit",
                schema: "coursemanagement");
        }
    }
}
