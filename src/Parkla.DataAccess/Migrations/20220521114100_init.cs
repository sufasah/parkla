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
            migrationBuilder.CreateTable(
                name: "cities",
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
                name: "districts",
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
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    wallet = table.Column<float>(type: "real", nullable: false),
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
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_districts_district_id",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "parks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    extras = table.Column<string[]>(type: "text[]", nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    empty_space = table.Column<int>(type: "integer", nullable: false),
                    occupied_space = table.Column<int>(type: "integer", nullable: false),
                    min_price = table.Column<float>(type: "real", nullable: true),
                    avarage_price = table.Column<float>(type: "real", nullable: true),
                    max_price = table.Column<float>(type: "real", nullable: true),
                    reserved_space = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parks", x => x.id);
                    table.ForeignKey(
                        name: "FK_parks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "park_areas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    park_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    template_image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reservations_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    empty_space = table.Column<int>(type: "integer", nullable: false),
                    occupied_space = table.Column<int>(type: "integer", nullable: false),
                    min_price = table.Column<float>(type: "real", nullable: true),
                    avarage_price = table.Column<float>(type: "real", nullable: true),
                    max_price = table.Column<float>(type: "real", nullable: true),
                    reserved_space = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_park_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_park_areas_parks_park_id",
                        column: x => x.park_id,
                        principalTable: "parks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "park_spaces",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    area_id = table.Column<int>(type: "integer", nullable: false),
                    real_space_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "3"),
                    space_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_park_spaces", x => x.id);
                    table.ForeignKey(
                        name: "FK_park_spaces_park_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "park_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pricings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    area_id = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricings", x => x.id);
                    table.ForeignKey(
                        name: "FK_pricings_park_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "park_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_park_spaces",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    park_id = table.Column<Guid>(type: "uuid", nullable: false),
                    space_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_real_park_spaces", x => x.id);
                    table.ForeignKey(
                        name: "FK_real_park_spaces_park_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "park_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_real_park_spaces_parks_park_id",
                        column: x => x.park_id,
                        principalTable: "parks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
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
                    table.PrimaryKey("PK_reservations", x => x.id);
                    table.ForeignKey(
                        name: "FK_reservations_park_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "park_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservations_pricings_pricing_id",
                        column: x => x.pricing_id,
                        principalTable: "pricings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "received_space_statusses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    space_id = table.Column<int>(type: "integer", nullable: true),
                    real_space_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "3"),
                    datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_received_space_statusses", x => x.id);
                    table.ForeignKey(
                        name: "FK_received_space_statusses_park_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "park_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_received_space_statusses_real_park_spaces_real_space_id",
                        column: x => x.real_space_id,
                        principalTable: "real_park_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_city_id",
                table: "districts",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_park_areas_park_id",
                table: "park_areas",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_park_spaces_area_id",
                table: "park_spaces",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_parks_user_id",
                table: "parks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricings_area_id",
                table: "pricings",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_real_park_spaces_park_id",
                table: "real_park_spaces",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_real_park_spaces_space_id",
                table: "real_park_spaces",
                column: "space_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_received_space_statusses_real_space_id",
                table: "received_space_statusses",
                column: "real_space_id");

            migrationBuilder.CreateIndex(
                name: "IX_received_space_statusses_space_id",
                table: "received_space_statusses",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_pricing_id",
                table: "reservations",
                column: "pricing_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_space_id",
                table: "reservations",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_user_id",
                table: "reservations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_city_id",
                table: "users",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_district_id",
                table: "users",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "received_space_statusses");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "real_park_spaces");

            migrationBuilder.DropTable(
                name: "pricings");

            migrationBuilder.DropTable(
                name: "park_spaces");

            migrationBuilder.DropTable(
                name: "park_areas");

            migrationBuilder.DropTable(
                name: "parks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "cities");
        }
    }
}
