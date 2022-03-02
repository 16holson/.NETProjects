using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class studentformfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentForms_ScholarshipAwards_StudentAwardId",
                table: "StudentForms");

            migrationBuilder.DropIndex(
                name: "IX_StudentForms_StudentAwardId",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "StudentAwardId",
                table: "StudentForms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentAwardId",
                table: "StudentForms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentForms_StudentAwardId",
                table: "StudentForms",
                column: "StudentAwardId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentForms_ScholarshipAwards_StudentAwardId",
                table: "StudentForms",
                column: "StudentAwardId",
                principalTable: "ScholarshipAwards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
