using Microsoft.EntityFrameworkCore.Migrations;

namespace TestSite.Migrations
{
    public partial class AddCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "SubComments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "MainCommnets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creator",
                table: "SubComments");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "MainCommnets");
        }
    }
}
