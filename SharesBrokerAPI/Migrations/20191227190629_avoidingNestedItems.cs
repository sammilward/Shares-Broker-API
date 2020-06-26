using Microsoft.EntityFrameworkCore.Migrations;

namespace SharesBrokerAPI.Migrations
{
    public partial class avoidingNestedItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Shares_ShareCompanySymbol",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_Username",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Shares_ShareCompanySymbol",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_Username",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ShareCompanySymbol",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Username",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ShareCompanySymbol",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_Username",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ShareCompanySymbol",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ShareCompanySymbol",
                table: "Purchases");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Sales",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "companySymbol",
                table: "Sales",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Purchases",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanySymbol",
                table: "Purchases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companySymbol",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CompanySymbol",
                table: "Purchases");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareCompanySymbol",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareCompanySymbol",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ShareCompanySymbol",
                table: "Sales",
                column: "ShareCompanySymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Username",
                table: "Sales",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShareCompanySymbol",
                table: "Purchases",
                column: "ShareCompanySymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_Username",
                table: "Purchases",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Shares_ShareCompanySymbol",
                table: "Purchases",
                column: "ShareCompanySymbol",
                principalTable: "Shares",
                principalColumn: "CompanySymbol",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_Username",
                table: "Purchases",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Shares_ShareCompanySymbol",
                table: "Sales",
                column: "ShareCompanySymbol",
                principalTable: "Shares",
                principalColumn: "CompanySymbol",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_Username",
                table: "Sales",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
