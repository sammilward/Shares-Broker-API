using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class avoidingNestedItemsForUserShares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserShares_Shares_ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShares_Users_Username",
                table: "UserShares");

            migrationBuilder.DropIndex(
                name: "IX_UserShares_ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.DropIndex(
                name: "IX_UserShares_Username",
                table: "UserShares");

            migrationBuilder.DropColumn(
                name: "ShareCompanySymbol",
                table: "UserShares");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserShares",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanySymbol",
                table: "UserShares",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanySymbol",
                table: "UserShares");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserShares",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareCompanySymbol",
                table: "UserShares",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_ShareCompanySymbol",
                table: "UserShares",
                column: "ShareCompanySymbol");

            migrationBuilder.CreateIndex(
                name: "IX_UserShares_Username",
                table: "UserShares",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_UserShares_Shares_ShareCompanySymbol",
                table: "UserShares",
                column: "ShareCompanySymbol",
                principalTable: "Shares",
                principalColumn: "CompanySymbol",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShares_Users_Username",
                table: "UserShares",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
