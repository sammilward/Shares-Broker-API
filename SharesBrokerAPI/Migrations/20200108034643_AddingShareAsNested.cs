using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class AddingShareAsNested : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanySymbol",
                table: "UserShares");

            migrationBuilder.AddColumn<string>(
                name: "ShareCompanySymbol",
                table: "UserShares",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_ShareCompanySymbol",
                table: "UserShares",
                column: "ShareCompanySymbol");

            migrationBuilder.AddForeignKey(
                name: "FK_UserShares_Shares_ShareCompanySymbol",
                table: "UserShares",
                column: "ShareCompanySymbol",
                principalTable: "Shares",
                principalColumn: "CompanySymbol",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserShares_Shares_ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.DropIndex(
                name: "IX_UserShares_ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.DropColumn(
                name: "ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.AddColumn<string>(
                name: "CompanySymbol",
                table: "UserShares",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
