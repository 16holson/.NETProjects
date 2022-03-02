using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class irecycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormCriteria_Forms_FormId",
                table: "FormCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_FormFields_Forms_FormId",
                table: "FormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormRequirements_Forms_FormId",
                table: "FormRequirements");

            migrationBuilder.AddColumn<bool>(
                name: "Recycled",
                table: "Forms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormRequirements",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "Recycled",
                table: "FormRequirements",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormFields",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "Recycled",
                table: "FormFields",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormCriteria",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "Recycled",
                table: "FormCriteria",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Recycled",
                table: "AwardCycles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RecycleBin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemDependency = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: false),
                    ItemName = table.Column<string>(nullable: false),
                    ItemType = table.Column<int>(nullable: false),
                    RecycledBy = table.Column<string>(nullable: true),
                    RecycledDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecycleBin", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FormCriteria_Forms_FormId",
                table: "FormCriteria",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_Forms_FormId",
                table: "FormFields",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequirements_Forms_FormId",
                table: "FormRequirements",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormCriteria_Forms_FormId",
                table: "FormCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_FormFields_Forms_FormId",
                table: "FormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormRequirements_Forms_FormId",
                table: "FormRequirements");

            migrationBuilder.DropTable(
                name: "RecycleBin");

            migrationBuilder.DropColumn(
                name: "Recycled",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Recycled",
                table: "FormRequirements");

            migrationBuilder.DropColumn(
                name: "Recycled",
                table: "FormFields");

            migrationBuilder.DropColumn(
                name: "Recycled",
                table: "FormCriteria");

            migrationBuilder.DropColumn(
                name: "Recycled",
                table: "AwardCycles");

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormRequirements",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormFields",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "FormCriteria",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormCriteria_Forms_FormId",
                table: "FormCriteria",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_Forms_FormId",
                table: "FormFields",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequirements_Forms_FormId",
                table: "FormRequirements",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
