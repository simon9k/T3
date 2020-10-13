using Microsoft.EntityFrameworkCore.Migrations;

namespace T3.Data.Migrations
{
    public partial class CourseUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginCourseID",
                table: "Course",
                newName: "OriginCourseId");

            migrationBuilder.AlterColumn<int>(
                name: "OriginCourseId",
                table: "Course",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginCourseId",
                table: "Course",
                newName: "OriginCourseID");

            migrationBuilder.AlterColumn<int>(
                name: "OriginCourseID",
                table: "Course",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
