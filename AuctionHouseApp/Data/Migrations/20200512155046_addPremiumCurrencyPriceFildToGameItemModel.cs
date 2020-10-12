using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class addPremiumCurrencyPriceFildToGameItemModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PremiumCurrencyPrice",
                table: "GameItemsDb",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumCurrencyPrice",
                table: "GameItemsDb");
        }
    }
}
