using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_pricings_pricing_id",
                schema: "public",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_reservations_pricing_id",
                schema: "public",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "pricing_id",
                schema: "public",
                table: "reservations");

            migrationBuilder.AddColumn<int>(
                name: "pricing_id",
                schema: "public",
                table: "park_spaces",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_park_spaces_pricing_id",
                schema: "public",
                table: "park_spaces",
                column: "pricing_id");

            migrationBuilder.AddForeignKey(
                name: "FK_park_spaces_pricings_pricing_id",
                schema: "public",
                table: "park_spaces",
                column: "pricing_id",
                principalSchema: "public",
                principalTable: "pricings",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_park_spaces_pricings_pricing_id",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.DropIndex(
                name: "IX_park_spaces_pricing_id",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.DropColumn(
                name: "pricing_id",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.AddColumn<int>(
                name: "pricing_id",
                schema: "public",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_pricing_id",
                schema: "public",
                table: "reservations",
                column: "pricing_id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_pricings_pricing_id",
                schema: "public",
                table: "reservations",
                column: "pricing_id",
                principalSchema: "public",
                principalTable: "pricings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
