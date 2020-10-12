using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class AddGameItemDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameItemsDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    LevelRequired = table.Column<int>(nullable: false),
                    Image = table.Column<string>(nullable: false),
                    SubCategoryId = table.Column<int>(nullable: false),
                    ItemQualityId = table.Column<int>(nullable: false),
                    ForWhichClassItemId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameItemsDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameItemsDb_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GameItemsDb_ForWhichClassItemDb_ForWhichClassItemId",
                        column: x => x.ForWhichClassItemId,
                        principalTable: "ForWhichClassItemDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GameItemsDb_ItemQualityDb_ItemQualityId",
                        column: x => x.ItemQualityId,
                        principalTable: "ItemQualityDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GameItemsDb_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameItemsDb_CategoryId",
                table: "GameItemsDb",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameItemsDb_ForWhichClassItemId",
                table: "GameItemsDb",
                column: "ForWhichClassItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GameItemsDb_ItemQualityId",
                table: "GameItemsDb",
                column: "ItemQualityId");

            migrationBuilder.CreateIndex(
                name: "IX_GameItemsDb_SubCategoryId",
                table: "GameItemsDb",
                column: "SubCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameItemsDb");
        }
    }
}
