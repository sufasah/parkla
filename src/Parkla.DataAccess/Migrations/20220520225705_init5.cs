using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reserved_space",
                schema: "public",
                table: "parks");

            migrationBuilder.DropColumn(
                name: "reserved_space",
                schema: "public",
                table: "park_areas");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "real_park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "real_park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "reserved_space",
                schema: "public",
                table: "parks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "reserved_space",
                schema: "public",
                table: "park_areas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
