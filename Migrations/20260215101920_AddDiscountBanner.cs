using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yash_Gems___Jewelleries.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Data transformation: Map existing string values to enum integers
            // 1 = NewIn, 2 = Promotional
            migrationBuilder.Sql("UPDATE Banners SET BannerType = '2' WHERE BannerType = 'Promotional'");
            migrationBuilder.Sql("UPDATE Banners SET BannerType = '1' WHERE BannerType IN ('Main', 'NewIn', 'NewLaunch') OR BannerType NOT IN ('1', '2')");

            migrationBuilder.AlterColumn<int>(
                name: "BannerType",
                table: "Banners",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "DiscountSchemeId",
                table: "Banners",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PricesWithTax = table.Column<bool>(type: "bit", nullable: false),
                    DefaultTaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_DiscountSchemeId",
                table: "Banners",
                column: "DiscountSchemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_DiscountSchemes_DiscountSchemeId",
                table: "Banners",
                column: "DiscountSchemeId",
                principalTable: "DiscountSchemes",
                principalColumn: "DiscountSchemeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_DiscountSchemes_DiscountSchemeId",
                table: "Banners");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_Banners_DiscountSchemeId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "DiscountSchemeId",
                table: "Banners");

            migrationBuilder.AlterColumn<string>(
                name: "BannerType",
                table: "Banners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Revert integers to strings
            migrationBuilder.Sql("UPDATE Banners SET BannerType = 'NewIn' WHERE BannerType = '1'");
            migrationBuilder.Sql("UPDATE Banners SET BannerType = 'Promotional' WHERE BannerType = '2'");
        }
    }
}
