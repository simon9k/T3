using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace T3.Data.Migrations
{
    public partial class addClientRoleInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    NickName = table.Column<string>(maxLength: 20, nullable: false),
                    Sex = table.Column<string>(nullable: true),
                    BOD = table.Column<DateTime>(nullable: false),
                    EOD = table.Column<DateTime>(nullable: false),
                    PersonalID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientID);
                    table.ForeignKey(
                        name: "FK_Client_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginCourseID = table.Column<int>(nullable: false),
                    TanentId = table.Column<Guid>(nullable: false),
                    IsCyclic = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Course_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseAssignment",
                columns: table => new
                {
                    AppUserId = table.Column<string>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    bPrimary = table.Column<bool>(nullable: false),
                    FinishState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssignment", x => new { x.CourseId, x.AppUserId });
                    table.ForeignKey(
                        name: "FK_CourseAssignment_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    FinishState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => new { x.CourseId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_Enrollment_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollment_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_TenantId_NickName",
                table: "Client",
                columns: new[] { "TenantId", "NickName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_TenantId",
                table: "Course",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_AppUserId",
                table: "CourseAssignment",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_ClientId",
                table: "Enrollment",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAssignment");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
