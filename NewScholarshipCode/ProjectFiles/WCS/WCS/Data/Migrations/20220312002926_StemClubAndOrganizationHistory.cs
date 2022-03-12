using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WCS.Data.Migrations
{
    public partial class StemClubAndOrganizationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StemClubAndOrganizationHistory",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StemClubAndOrganizationHistory",
                table: "StudentProfiles");
        }
    }
}
