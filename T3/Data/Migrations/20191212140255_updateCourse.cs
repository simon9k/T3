using Microsoft.EntityFrameworkCore.Migrations;

namespace T3.Data.Migrations
{
    public partial class updateCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientID",
                table: "Client",
                newName: "ClientId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Client",
                newName: "ClientID");
        }
    }
}
