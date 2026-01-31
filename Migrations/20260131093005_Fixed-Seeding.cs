using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Yash_Gems___Jewelleries.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 1,
                column: "Description",
                value: "India's most trusted jewelry brand");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 2,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "Malabar Gold", "Heritage jewelry designs" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 3,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "CaratLane", "Modern jewelry designs" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 4,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "Giva", "Affordable silver jewelry" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 5,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "Yash", "Premium in-house brand" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Rings", "Finger rings for all occasions" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Necklaces", "Elegant necklaces and chains" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Earrings", "Studs, jhumkas and drops" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bangles", "Traditional and modern bangles" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bracelets", "Wrist jewelry for men and women" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedDate", "Description", "ImageUrl", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { 6, "Nose Pins", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Traditional nose pins", null, true, null },
                    { 7, "Mangalsutras", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Symbol of marriage", null, true, null },
                    { 8, "Toe Rings", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Traditional toe rings", null, true, null },
                    { 9, "Anklets", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Payal and anklets", null, true, null }
                });

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 1,
                columns: new[] { "CertifyType", "IssuingAuthority" },
                values: new object[] { "BIS 916", "Bureau of Indian Standards (HM)" });

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 3,
                columns: new[] { "CertifyType", "IssuingAuthority" },
                values: new object[] { "GIA", "Gemological Institute of America" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 1,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Very Very Slightly Included", "D-H", "VVS" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 2,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Very Slightly Included", "G-J", "VS" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 3,
                columns: new[] { "Clarity", "Color", "Cut", "QualityGrade" },
                values: new object[] { "Slightly Included", "H-K", "Good", "SI" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 4,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Artificial (American Diamond)", "White", "AD" });

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 1,
                column: "Description",
                value: "Pure Gold (Coins/Bars)");

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 2,
                column: "Description",
                value: "Standard for Gold Jewelry");

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 3,
                column: "Description",
                value: "Standard for Diamond Jewelry");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 1,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Classic gold jewelry", "Gold Jewelry" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 2,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Sparkling diamond pieces", "Diamond Jewelry" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 3,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Rare and pure platinum", "Platinum" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 4,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Sterling silver jewelry", "Silver" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 5,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Traditional Kundan jewelry", "Kundan" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 6,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Uncut diamond jewelry", "Polki" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 7,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "South Indian temple designs", "Temple Jewelry" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 8,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Precious stone jewelry", "Gemstone" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 1,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Red", "Standard" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 2,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Green", "Standard" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 3,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Blue", "Standard" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 5,
                columns: new[] { "Color", "Origin", "QualityGrade", "StoneType" },
                values: new object[] { "Blue", "Brazil", "A", "Topaz" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 6,
                columns: new[] { "Color", "StoneType" },
                values: new object[] { "Purple", "Amethyst" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 7,
                columns: new[] { "Color", "Origin", "QualityGrade", "StoneType" },
                values: new object[] { "Multicolor", "India", "Standard", "Meena" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 1,
                column: "Description",
                value: "Pakistan's most trusted jewelry brand");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 2,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "CaratLane", "Modern jewelry designs" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 3,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "Mia by Tanishq", "Contemporary jewelry for millennials" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 4,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "PC Jeweller", "Traditional and modern designs" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 5,
                columns: new[] { "BrandType", "Description" },
                values: new object[] { "Kalyan Jewellers", "South Indian traditional jewelry" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Gold", "Gold jewelry collection" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Diamond", "Diamond jewelry collection" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Platinum", "Platinum jewelry collection" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Silver", "Silver jewelry collection" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Gemstone", "Gemstone jewelry collection" });

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 1,
                columns: new[] { "CertifyType", "IssuingAuthority" },
                values: new object[] { "GIA", "Gemological Institute of America" });

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "CertificateId",
                keyValue: 3,
                columns: new[] { "CertifyType", "IssuingAuthority" },
                values: new object[] { "AGS", "American Gem Society" });

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "CertificateId", "CertifyType", "CreatedDate", "Description", "IsActive", "IssuingAuthority", "ModifiedDate" },
                values: new object[,]
                {
                    { 4, "HRD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "HRD Antwerp", null },
                    { 5, "BIS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Bureau of Indian Standards", null }
                });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 1,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Internally Flawless", "D-F", "IF" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 2,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Very Very Slightly Included 1", "D-G", "VVS1" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 3,
                columns: new[] { "Clarity", "Color", "Cut", "QualityGrade" },
                values: new object[] { "Very Very Slightly Included 2", "E-H", "Very Good", "VVS2" });

            migrationBuilder.UpdateData(
                table: "DiamondQualities",
                keyColumn: "DiamondQualityId",
                keyValue: 4,
                columns: new[] { "Clarity", "Color", "QualityGrade" },
                values: new object[] { "Very Slightly Included 1", "F-I", "VS1" });

            migrationBuilder.InsertData(
                table: "DiamondQualities",
                columns: new[] { "DiamondQualityId", "Clarity", "Color", "CreatedDate", "Cut", "Description", "IsActive", "ModifiedDate", "QualityGrade" },
                values: new object[,]
                {
                    { 5, "Very Slightly Included 2", "G-J", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, true, null, "VS2" },
                    { 6, "Slightly Included 1", "H-K", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Good", null, true, null, "SI1" },
                    { 7, "Slightly Included 2", "I-L", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fair", null, true, null, "SI2" }
                });

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 1,
                column: "Description",
                value: "Pure Gold");

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 2,
                column: "Description",
                value: "Most common in Pakistan");

            migrationBuilder.UpdateData(
                table: "GoldKarats",
                keyColumn: "GoldTypeId",
                keyValue: 3,
                column: "Description",
                value: "Durable for jewelry");

            migrationBuilder.InsertData(
                table: "GoldKarats",
                columns: new[] { "GoldTypeId", "CreatedDate", "Description", "GoldCarat", "IsActive", "ModifiedDate", "PurityPercentage" },
                values: new object[] { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Strong and affordable", "14K", true, null, 58.3m });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 1,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Finger rings", "Ring" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 2,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Necklaces and chains", "Necklace" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 3,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Ear jewelry", "Earrings" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 4,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Wrist jewelry", "Bracelet" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 5,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Bangles and kada", "Bangle" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 6,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Pendants and lockets", "Pendant" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 7,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Traditional mangalsutra", "Mangalsutra" });

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "ProductTypeId",
                keyValue: 8,
                columns: new[] { "Description", "ProductTypeName" },
                values: new object[] { "Nose jewelry", "Nose Pin" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 1,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Pigeon Blood Red", "AA+" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 2,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Vivid Green", "AA+" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 3,
                columns: new[] { "Color", "QualityGrade" },
                values: new object[] { "Royal Blue", "AA" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 5,
                columns: new[] { "Color", "Origin", "QualityGrade", "StoneType" },
                values: new object[] { "Violet Blue", "Tanzania", "AA", "Tanzanite" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 6,
                columns: new[] { "Color", "StoneType" },
                values: new object[] { "Blue", "Topaz" });

            migrationBuilder.UpdateData(
                table: "StoneQualities",
                keyColumn: "StoneQualityId",
                keyValue: 7,
                columns: new[] { "Color", "Origin", "QualityGrade", "StoneType" },
                values: new object[] { "Purple", "Brazil", "A", "Amethyst" });
        }
    }
}
