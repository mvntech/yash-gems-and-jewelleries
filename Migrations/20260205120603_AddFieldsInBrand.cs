using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yash_Gems___Jewelleries.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsInBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Brands",
                newName: "ModifiedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Brands",
                newName: "UpdatedDate");
        }
    }
}
