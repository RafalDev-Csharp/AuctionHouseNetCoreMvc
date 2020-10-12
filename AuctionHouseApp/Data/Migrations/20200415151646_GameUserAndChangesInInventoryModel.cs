using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class GameUserAndChangesInInventoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "InventoryDb");

            migrationBuilder.AddColumn<string>(
                name: "GameUserId",
                table: "InventoryDb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cash",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameUserId",
                table: "InventoryDb");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "InventoryDb",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
