using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class AddedUserShares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ShareCompanySymbol = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShares_Shares_ShareCompanySymbol",
                        column: x => x.ShareCompanySymbol,
                        principalTable: "Shares",
                        principalColumn: "CompanySymbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserShares_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_ShareCompanySymbol",
                table: "UserShares",
                column: "ShareCompanySymbol");

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_Username",
                table: "UserShares",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserShares");
        }
    }
}
