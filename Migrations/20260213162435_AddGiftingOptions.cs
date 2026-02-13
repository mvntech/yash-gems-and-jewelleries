using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yash_Gems___Jewelleries.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftingOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GiftMessage",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGift",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiftMessage",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsGift",
                table: "Orders");
        }
    }
}
