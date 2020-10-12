using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouseApp.Data.Migrations
{
    public partial class AddTimeSpan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeRemained",
                table: "SellItemDb",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeRemained",
                table: "SellItemDb");
        }
    }
}
