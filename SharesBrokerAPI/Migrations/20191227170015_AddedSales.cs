using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class AddedSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SaleTime = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ShareCompanySymbol = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    TotalValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Shares_ShareCompanySymbol",
                        column: x => x.ShareCompanySymbol,
                        principalTable: "Shares",
                        principalColumn: "CompanySymbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sales_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ShareCompanySymbol",
                table: "Sales",
                column: "ShareCompanySymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Username",
                table: "Sales",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");
        }
    }
}
