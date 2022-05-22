using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "old_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "new_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "space_name",
                schema: "public",
                table: "received_space_statusses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "space_name",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.AlterColumn<int>(
                name: "old_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "new_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
