using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "public",
                table: "real_park_spaces");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "public",
                table: "parks");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "public",
                table: "park_areas");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "public",
                table: "real_park_spaces",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "public",
                table: "parks",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "public",
                table: "park_spaces",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "public",
                table: "park_areas",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "public",
                table: "real_park_spaces");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "public",
                table: "parks");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "public",
                table: "park_areas");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "public",
                table: "real_park_spaces",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "public",
                table: "parks",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "public",
                table: "park_spaces",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "public",
                table: "park_areas",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }
    }
}
