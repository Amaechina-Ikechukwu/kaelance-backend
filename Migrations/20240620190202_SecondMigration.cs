using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kallum.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "102352de-c6d8-4c05-b529-b2027095066a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e157b54c-dff8-4be2-abee-5093d9ce23b6");

            migrationBuilder.AddColumn<int>(
                name: "BalanceDetailsId",
                table: "UserAccountsData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BankAccountId",
                table: "UserAccountsData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KallumLockData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SecurePin = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionPin = table.Column<int>(type: "INTEGER", nullable: false),
                    UserAccountId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KallumLockData", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bda0a83-de38-4910-9317-dbfada504813", null, "User", "USER" },
                    { "39447457-20d4-4645-91d8-651f1bd5900f", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountsData_BalanceDetailsId",
                table: "UserAccountsData",
                column: "BalanceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountsData_BankAccountId",
                table: "UserAccountsData",
                column: "BankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccountsData_BalanceDetailsData_BalanceDetailsId",
                table: "UserAccountsData",
                column: "BalanceDetailsId",
                principalTable: "BalanceDetailsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccountsData_BankAccountsData_BankAccountId",
                table: "UserAccountsData",
                column: "BankAccountId",
                principalTable: "BankAccountsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccountsData_BalanceDetailsData_BalanceDetailsId",
                table: "UserAccountsData");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccountsData_BankAccountsData_BankAccountId",
                table: "UserAccountsData");

            migrationBuilder.DropTable(
                name: "KallumLockData");

            migrationBuilder.DropIndex(
                name: "IX_UserAccountsData_BalanceDetailsId",
                table: "UserAccountsData");

            migrationBuilder.DropIndex(
                name: "IX_UserAccountsData_BankAccountId",
                table: "UserAccountsData");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bda0a83-de38-4910-9317-dbfada504813");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39447457-20d4-4645-91d8-651f1bd5900f");

            migrationBuilder.DropColumn(
                name: "BalanceDetailsId",
                table: "UserAccountsData");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "UserAccountsData");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "102352de-c6d8-4c05-b529-b2027095066a", null, "Admin", "ADMIN" },
                    { "e157b54c-dff8-4be2-abee-5093d9ce23b6", null, "User", "USER" }
                });
        }
    }
}
