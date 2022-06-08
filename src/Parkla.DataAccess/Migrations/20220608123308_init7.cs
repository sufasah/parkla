using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "parks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "park_areas");

            migrationBuilder.RenameColumn(
                name: "avarage_price",
                schema: "public",
                table: "parks",
                newName: "average_price");

            migrationBuilder.RenameColumn(
                name: "avarage_price",
                schema: "public",
                table: "park_areas",
                newName: "average_price");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "parks",
                sql: "min_price <= average_price and average_price <= max_price");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "park_areas",
                sql: "min_price <= average_price and average_price <= max_price");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "parks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "park_areas");

            migrationBuilder.RenameColumn(
                name: "average_price",
                schema: "public",
                table: "parks",
                newName: "avarage_price");

            migrationBuilder.RenameColumn(
                name: "average_price",
                schema: "public",
                table: "park_areas",
                newName: "avarage_price");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "parks",
                sql: "min_price <= avarage_price and avarage_price <= max_price");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PRICES_VALID",
                schema: "public",
                table: "park_areas",
                sql: "min_price <= avarage_price and avarage_price <= max_price");
        }
    }
}
