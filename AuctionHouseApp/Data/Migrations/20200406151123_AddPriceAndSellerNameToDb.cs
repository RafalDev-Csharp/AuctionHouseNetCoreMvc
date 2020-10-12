using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class AddPriceAndSellerNameToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "SellItemDb",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "SellItemDb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "SellItemDb");

            migrationBuilder.DropColumn(
                name: "SellerName",
                table: "SellItemDb");
        }
    }
}
