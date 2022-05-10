using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "cities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    latitude = table.Column<double>(type: "double precision", precision: 2, scale: 15, nullable: false),
                    longitude = table.Column<double>(type: "double precision", precision: 2, scale: 15, nullable: false),
                    extras = table.Column<string[]>(type: "text[]", maxLength: 10, nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    empty_space = table.Column<int>(type: "integer", nullable: false),
                    reserved_space = table.Column<int>(type: "integer", nullable: false),
                    occupied_space = table.Column<int>(type: "integer", nullable: false),
                    min_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false),
                    avarage_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false),
                    max_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parks", x => x.id);
                    table.CheckConstraint("CK_LATITUDE_AND_LONGITUDE_ARE_VALID", "latitude >= -90 and latitude <= 90 and longitude >= -180 and longitude <= 180");
                    table.CheckConstraint("CK_PRICES_VALID", "min_price <= avarage_price and avarage_price <= max_price");
                    table.CheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                });

            migrationBuilder.CreateTable(
                name: "districts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    city_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.id);
                    table.ForeignKey(
                        name: "FK_districts_cities_city_id",
                        column: x => x.city_id,
                        principalSchema: "public",
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "park_areas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    park_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    template_image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reservations_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    empty_space = table.Column<int>(type: "integer", nullable: false),
                    reserved_space = table.Column<int>(type: "integer", nullable: false),
                    occupied_space = table.Column<int>(type: "integer", nullable: false),
                    min_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false),
                    avarage_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false),
                    max_price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_park_areas", x => x.id);
                    table.CheckConstraint("CK_PRICES_VALID", "min_price <= avarage_price and avarage_price <= max_price");
                    table.CheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                    table.ForeignKey(
                        name: "FK_park_areas_parks_park_id",
                        column: x => x.park_id,
                        principalSchema: "public",
                        principalTable: "parks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    wallet = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false),
                    username = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    password = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: true),
                    verify_code = table.Column<string>(type: "text", nullable: true),
                    refresh_token_signature = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    district_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_cities_city_id",
                        column: x => x.city_id,
                        principalSchema: "public",
                        principalTable: "cities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_users_districts_district_id",
                        column: x => x.district_id,
                        principalSchema: "public",
                        principalTable: "districts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "park_spaces",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    area_id = table.Column<int>(type: "integer", nullable: false),
                    real_space_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    space_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_park_spaces", x => x.id);
                    table.CheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                    table.ForeignKey(
                        name: "FK_park_spaces_park_areas_area_id",
                        column: x => x.area_id,
                        principalSchema: "public",
                        principalTable: "park_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pricings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    area_id = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<float>(type: "real", precision: 30, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricings", x => x.id);
                    table.ForeignKey(
                        name: "FK_pricings_park_areas_area_id",
                        column: x => x.area_id,
                        principalSchema: "public",
                        principalTable: "park_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_park_spaces",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    park_id = table.Column<int>(type: "integer", nullable: false),
                    space_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_real_park_spaces", x => x.id);
                    table.CheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                    table.ForeignKey(
                        name: "FK_real_park_spaces_park_spaces_space_id",
                        column: x => x.space_id,
                        principalSchema: "public",
                        principalTable: "park_spaces",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_real_park_spaces_parks_park_id",
                        column: x => x.park_id,
                        principalSchema: "public",
                        principalTable: "parks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservaions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    space_id = table.Column<int>(type: "integer", nullable: false),
                    pricing_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservaions", x => x.id);
                    table.CheckConstraint("CK_STARTTIME_LESS_THAN_ENDTIME", "start_time < end_time");
                    table.ForeignKey(
                        name: "FK_reservaions_park_spaces_space_id",
                        column: x => x.space_id,
                        principalSchema: "public",
                        principalTable: "park_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservaions_pricings_pricing_id",
                        column: x => x.pricing_id,
                        principalSchema: "public",
                        principalTable: "pricings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservaions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "received_space_statusses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    space_id = table.Column<int>(type: "integer", nullable: true),
                    real_space_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_received_space_statusses", x => x.id);
                    table.CheckConstraint("CK_DATETIME_LESS_THAN_NOW_UTC", "datetime < (now() at time zone 'utc')");
                    table.ForeignKey(
                        name: "FK_received_space_statusses_park_spaces_space_id",
                        column: x => x.space_id,
                        principalSchema: "public",
                        principalTable: "park_spaces",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                        column: x => x.real_space_id,
                        principalSchema: "public",
                        principalTable: "real_park_spaces",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_city_id",
                schema: "public",
                table: "districts",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_park_areas_park_id",
                schema: "public",
                table: "park_areas",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_park_spaces_area_id",
                schema: "public",
                table: "park_spaces",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricings_area_id",
                schema: "public",
                table: "pricings",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_real_park_spaces_park_id",
                schema: "public",
                table: "real_park_spaces",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_real_park_spaces_space_id",
                schema: "public",
                table: "real_park_spaces",
                column: "space_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_received_space_statusses_real_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "real_space_id");

            migrationBuilder.CreateIndex(
                name: "IX_received_space_statusses_space_id",
                schema: "public",
                table: "received_space_statusses",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservaions_pricing_id",
                schema: "public",
                table: "reservaions",
                column: "pricing_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservaions_space_id",
                schema: "public",
                table: "reservaions",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservaions_user_id",
                schema: "public",
                table: "reservaions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_city_id",
                schema: "public",
                table: "users",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_district_id",
                schema: "public",
                table: "users",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                schema: "public",
                table: "users",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "received_space_statusses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservaions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "real_park_spaces",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pricings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "park_spaces",
                schema: "public");

            migrationBuilder.DropTable(
                name: "districts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "park_areas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "parks",
                schema: "public");
        }
    }
}
