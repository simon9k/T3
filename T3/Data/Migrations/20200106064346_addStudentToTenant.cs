using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace T3.Data.Migrations
{
    public partial class addStudentToTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Tenants_TenantId",
                table: "Students");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Students",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Tenants_TenantId",
                table: "Students",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Tenants_TenantId",
                table: "Students");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Tenants_TenantId",
                table: "Students",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
