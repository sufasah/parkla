using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_DATETIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses",
                sql: "datetime <= now()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "real_park_spaces",
                sql: "status_update_time <= now()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "parks",
                sql: "status_update_time <= now()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "park_spaces",
                sql: "status_update_time <= now()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "park_areas",
                sql: "status_update_time <= now()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DATETIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "received_space_statusses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "real_park_spaces");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "parks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "park_spaces");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW",
                schema: "public",
                table: "park_areas");
        }
    }
}
