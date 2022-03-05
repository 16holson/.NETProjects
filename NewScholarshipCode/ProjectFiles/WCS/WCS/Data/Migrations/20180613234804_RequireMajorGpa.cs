using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class RequireMajorGpa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "MajorGPA",
                table: "StudentProfiles",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "MajorGPA",
                table: "StudentProfiles",
                nullable: true,
                oldClrType: typeof(float));
        }
    }
}
