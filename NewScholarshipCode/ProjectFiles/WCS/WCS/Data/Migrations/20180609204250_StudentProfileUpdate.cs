using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class StudentProfileUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "StudentProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "StudentProfiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "StudentProfiles");
        }
    }
}
