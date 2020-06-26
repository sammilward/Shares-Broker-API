using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class Purchases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PurchaseTime = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ShareCompanySymbol = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    TotalValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Shares_ShareCompanySymbol",
                        column: x => x.ShareCompanySymbol,
                        principalTable: "Shares",
                        principalColumn: "CompanySymbol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShareCompanySymbol",
                table: "Purchases",
                column: "ShareCompanySymbol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");
        }
    }
}
