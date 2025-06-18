using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherCoursesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("416ef558-c4fb-4abe-9525-7c3f00a889c7"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("8a7d5a75-2291-4ef6-9ba6-6cb7e16882f6"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("a50793cb-cceb-459c-a5af-738f0f4eaa09"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("fe38aba0-24f7-4157-8a1c-ed8b76c50f41"));

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Parents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TeacherCourses",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherCourses", x => new { x.TeacherId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_TeacherCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherCourses_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Code", "CreatedAt", "Credits", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("45d6159c-66d2-487b-941d-058540ca354d"), "SOC101", new DateTime(2025, 6, 18, 13, 43, 1, 331, DateTimeKind.Utc).AddTicks(6748), 3, "Introduction to social sciences", true, "Social Studies", null },
                    { new Guid("a3aac67d-2789-44da-8743-04e08369933a"), "ENG101", new DateTime(2025, 6, 18, 13, 43, 1, 331, DateTimeKind.Utc).AddTicks(6739), 3, "English language and literature", true, "English Language", null },
                    { new Guid("ca1bd0e0-6584-4456-87a7-fbd3eaaaf2ff"), "SCI101", new DateTime(2025, 6, 18, 13, 43, 1, 331, DateTimeKind.Utc).AddTicks(6736), 3, "Introduction to scientific principles", true, "Science", null },
                    { new Guid("f15e7079-b6f5-4e9e-9263-ffc7c607b023"), "MATH101", new DateTime(2025, 6, 18, 13, 43, 1, 331, DateTimeKind.Utc).AddTicks(6573), 3, "Introduction to basic mathematics concepts", true, "Mathematics", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_CourseId",
                table: "TeacherCourses",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherCourses");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("45d6159c-66d2-487b-941d-058540ca354d"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("a3aac67d-2789-44da-8743-04e08369933a"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("ca1bd0e0-6584-4456-87a7-fbd3eaaaf2ff"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("f15e7079-b6f5-4e9e-9263-ffc7c607b023"));

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Parents");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Code", "CreatedAt", "Credits", "Description", "IsActive", "Name", "TeacherId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("416ef558-c4fb-4abe-9525-7c3f00a889c7"), "SCI101", new DateTime(2025, 6, 12, 14, 26, 30, 210, DateTimeKind.Utc).AddTicks(5136), 3, "Introduction to scientific principles", true, "Science", null, null },
                    { new Guid("8a7d5a75-2291-4ef6-9ba6-6cb7e16882f6"), "MATH101", new DateTime(2025, 6, 12, 14, 26, 30, 210, DateTimeKind.Utc).AddTicks(4946), 3, "Introduction to basic mathematics concepts", true, "Mathematics", null, null },
                    { new Guid("a50793cb-cceb-459c-a5af-738f0f4eaa09"), "ENG101", new DateTime(2025, 6, 12, 14, 26, 30, 210, DateTimeKind.Utc).AddTicks(5140), 3, "English language and literature", true, "English Language", null, null },
                    { new Guid("fe38aba0-24f7-4157-8a1c-ed8b76c50f41"), "SOC101", new DateTime(2025, 6, 12, 14, 26, 30, 210, DateTimeKind.Utc).AddTicks(5143), 3, "Introduction to social sciences", true, "Social Studies", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
