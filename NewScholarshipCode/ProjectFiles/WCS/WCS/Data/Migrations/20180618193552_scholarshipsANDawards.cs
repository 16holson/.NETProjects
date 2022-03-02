using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WCS.Data.Migrations
{
    public partial class scholarshipsANDawards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwardMonies");

            migrationBuilder.DropTable(
                name: "MoneyPots");

            migrationBuilder.DropTable(
                name: "StudentAwards");

            migrationBuilder.AddColumn<int>(
                name: "StudentAwardId",
                table: "StudentForms",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Invites",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Invites",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeStudentRating",
                table: "Forms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Scholarships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipAwards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AwardCycleId = table.Column<int>(nullable: false),
                    ScholarshipId = table.Column<int>(nullable: false),
                    StudentProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScholarshipAwards_AwardCycles_AwardCycleId",
                        column: x => x.AwardCycleId,
                        principalTable: "AwardCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipAwards_Scholarships_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipAwards_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipFunds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(nullable: false),
                    AwardCycleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ScholarshipId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipFunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScholarshipFunds_AwardCycles_AwardCycleId",
                        column: x => x.AwardCycleId,
                        principalTable: "AwardCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipFunds_Scholarships_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipAwardMonies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(nullable: false),
                    ScholarshipAwardId = table.Column<int>(nullable: false),
                    ScholarshipFundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipAwardMonies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScholarshipAwardMonies_ScholarshipAwards_ScholarshipAwardId",
                        column: x => x.ScholarshipAwardId,
                        principalTable: "ScholarshipAwards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ScholarshipAwardMonies_ScholarshipFunds_ScholarshipFundId",
                        column: x => x.ScholarshipFundId,
                        principalTable: "ScholarshipFunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentForms_StudentAwardId",
                table: "StudentForms",
                column: "StudentAwardId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipAwardMonies_ScholarshipAwardId",
                table: "ScholarshipAwardMonies",
                column: "ScholarshipAwardId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipAwardMonies_ScholarshipFundId",
                table: "ScholarshipAwardMonies",
                column: "ScholarshipFundId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipAwards_AwardCycleId",
                table: "ScholarshipAwards",
                column: "AwardCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipAwards_ScholarshipId",
                table: "ScholarshipAwards",
                column: "ScholarshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipAwards_StudentProfileId",
                table: "ScholarshipAwards",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipFunds_AwardCycleId",
                table: "ScholarshipFunds",
                column: "AwardCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipFunds_ScholarshipId",
                table: "ScholarshipFunds",
                column: "ScholarshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentForms_ScholarshipAwards_StudentAwardId",
                table: "StudentForms",
                column: "StudentAwardId",
                principalTable: "ScholarshipAwards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentForms_ScholarshipAwards_StudentAwardId",
                table: "StudentForms");

            migrationBuilder.DropTable(
                name: "ScholarshipAwardMonies");

            migrationBuilder.DropTable(
                name: "ScholarshipAwards");

            migrationBuilder.DropTable(
                name: "ScholarshipFunds");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropIndex(
                name: "IX_StudentForms_StudentAwardId",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "StudentAwardId",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "IncludeStudentRating",
                table: "Forms");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Invites",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Invites",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "MoneyPots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(nullable: false),
                    AwardCycleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyPots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoneyPots_AwardCycles_AwardCycleId",
                        column: x => x.AwardCycleId,
                        principalTable: "AwardCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAwards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StudentFormId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAwards_StudentForms_StudentFormId",
                        column: x => x.StudentFormId,
                        principalTable: "StudentForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AwardMonies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(nullable: false),
                    MoneyPotId = table.Column<int>(nullable: false),
                    StudentAwardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwardMonies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AwardMonies_MoneyPots_MoneyPotId",
                        column: x => x.MoneyPotId,
                        principalTable: "MoneyPots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AwardMonies_StudentAwards_StudentAwardId",
                        column: x => x.StudentAwardId,
                        principalTable: "StudentAwards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AwardMonies_MoneyPotId",
                table: "AwardMonies",
                column: "MoneyPotId");

            migrationBuilder.CreateIndex(
                name: "IX_AwardMonies_StudentAwardId",
                table: "AwardMonies",
                column: "StudentAwardId");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyPots_AwardCycleId",
                table: "MoneyPots",
                column: "AwardCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAwards_StudentFormId",
                table: "StudentAwards",
                column: "StudentFormId",
                unique: true);
        }
    }
}
