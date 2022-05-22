using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DATETIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.RenameColumn(
                name: "datetime",
                schema: "public",
                table: "received_space_statusses",
                newName: "status_data_time");

            migrationBuilder.AddColumn<int>(
                name: "new_real_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "new_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "old_real_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "old_space_status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "real_space_name",
                schema: "public",
                table: "received_space_statusses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "received_time",
                schema: "public",
                table: "received_space_statusses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "real_park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RECEIVED_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses",
                sql: "received_time <= now()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_STATUS_DATA_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses",
                sql: "status_data_time <= now()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RECEIVED_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_STATUS_DATA_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "new_real_space_status",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "new_space_status",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "old_real_space_status",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "old_space_status",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "real_space_name",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropColumn(
                name: "received_time",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.RenameColumn(
                name: "status_data_time",
                schema: "public",
                table: "received_space_statusses",
                newName: "datetime");

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "public",
                table: "received_space_statusses",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "real_park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "public",
                table: "park_spaces",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.AddCheckConstraint(
                name: "CK_DATETIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses",
                sql: "datetime <= now()");
        }
    }
}
