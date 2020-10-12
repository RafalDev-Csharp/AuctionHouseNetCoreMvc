using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class AddWhichClassAndQualityClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForWhichClassItemDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForWhichClassItemDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemQualityDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemQualityDb", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForWhichClassItemDb");

            migrationBuilder.DropTable(
                name: "ItemQualityDb");
        }
    }
}
