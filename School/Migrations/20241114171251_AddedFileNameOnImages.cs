using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACM.Migrations
{
    public partial class AddedFileNameOnImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HouseImageName",
                table: "HouseImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseImageName",
                table: "HouseImages");
        }
    }
}
