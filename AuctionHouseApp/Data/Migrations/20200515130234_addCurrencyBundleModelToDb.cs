using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class addCurrencyBundleModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyBundleDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameCurrencyAmount = table.Column<int>(nullable: false),
                    BonusCurrency = table.Column<int>(nullable: false),
                    CashValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyBundleDb", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyBundleDb");
        }
    }
}
