using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_districts_cities_city_id",
                schema: "public",
                table: "districts");

            migrationBuilder.DropForeignKey(
                name: "FK_real_park_spaces_park_spaces_space_id",
                schema: "public",
                table: "real_park_spaces");

            migrationBuilder.DropForeignKey(
                name: "FK_received_space_statusses_park_spaces_space_id",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_city_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_districts_district_id",
                schema: "public",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "FK_districts_cities_city_id",
                schema: "public",
                table: "districts",
                column: "city_id",
                principalSchema: "public",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_real_park_spaces_park_spaces_space_id",
                schema: "public",
                table: "real_park_spaces",
                column: "space_id",
                principalSchema: "public",
                principalTable: "park_spaces",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_received_space_statusses_park_spaces_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "space_id",
                principalSchema: "public",
                principalTable: "park_spaces",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "real_space_id",
                principalSchema: "public",
                principalTable: "real_park_spaces",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_city_id",
                schema: "public",
                table: "users",
                column: "city_id",
                principalSchema: "public",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_districts_district_id",
                schema: "public",
                table: "users",
                column: "district_id",
                principalSchema: "public",
                principalTable: "districts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_districts_cities_city_id",
                schema: "public",
                table: "districts");

            migrationBuilder.DropForeignKey(
                name: "FK_real_park_spaces_park_spaces_space_id",
                schema: "public",
                table: "real_park_spaces");

            migrationBuilder.DropForeignKey(
                name: "FK_received_space_statusses_park_spaces_space_id",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_city_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_districts_district_id",
                schema: "public",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "FK_districts_cities_city_id",
                schema: "public",
                table: "districts",
                column: "city_id",
                principalSchema: "public",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_real_park_spaces_park_spaces_space_id",
                schema: "public",
                table: "real_park_spaces",
                column: "space_id",
                principalSchema: "public",
                principalTable: "park_spaces",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_received_space_statusses_park_spaces_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "space_id",
                principalSchema: "public",
                principalTable: "park_spaces",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "real_space_id",
                principalSchema: "public",
                principalTable: "real_park_spaces",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_city_id",
                schema: "public",
                table: "users",
                column: "city_id",
                principalSchema: "public",
                principalTable: "cities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_districts_district_id",
                schema: "public",
                table: "users",
                column: "district_id",
                principalSchema: "public",
                principalTable: "districts",
                principalColumn: "id");
        }
    }
}
