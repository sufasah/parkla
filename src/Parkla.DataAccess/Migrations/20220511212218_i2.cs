using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class i2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "min_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "max_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "avarage_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "public",
                table: "parks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "min_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "max_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "avarage_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_parks_UserId",
                schema: "public",
                table: "parks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_parks_users_UserId",
                schema: "public",
                table: "parks",
                column: "UserId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parks_users_UserId",
                schema: "public",
                table: "parks");

            migrationBuilder.DropIndex(
                name: "IX_parks_UserId",
                schema: "public",
                table: "parks");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "public",
                table: "parks");

            migrationBuilder.AlterColumn<float>(
                name: "min_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "max_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "avarage_price",
                schema: "public",
                table: "parks",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "min_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "max_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "avarage_price",
                schema: "public",
                table: "park_areas",
                type: "real",
                precision: 30,
                scale: 2,
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 30,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
