using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class FormRatingJudgeIdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRatings_AspNetUsers_JudgeId1",
                table: "FormRatings");

            migrationBuilder.DropIndex(
                name: "IX_FormRatings_JudgeId1",
                table: "FormRatings");

            migrationBuilder.DropColumn(
                name: "JudgeId1",
                table: "FormRatings");

            migrationBuilder.AlterColumn<string>(
                name: "JudgeId",
                table: "FormRatings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_FormRatings_JudgeId",
                table: "FormRatings",
                column: "JudgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRatings_AspNetUsers_JudgeId",
                table: "FormRatings",
                column: "JudgeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRatings_AspNetUsers_JudgeId",
                table: "FormRatings");

            migrationBuilder.DropIndex(
                name: "IX_FormRatings_JudgeId",
                table: "FormRatings");

            migrationBuilder.AlterColumn<int>(
                name: "JudgeId",
                table: "FormRatings",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JudgeId1",
                table: "FormRatings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRatings_JudgeId1",
                table: "FormRatings",
                column: "JudgeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRatings_AspNetUsers_JudgeId1",
                table: "FormRatings",
                column: "JudgeId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
