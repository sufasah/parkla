using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parks_users_UserId",
                schema: "public",
                table: "parks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "public",
                table: "parks",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_parks_UserId",
                schema: "public",
                table: "parks",
                newName: "IX_parks_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_parks_users_user_id",
                schema: "public",
                table: "parks",
                column: "user_id",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parks_users_user_id",
                schema: "public",
                table: "parks");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "public",
                table: "parks",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_parks_user_id",
                schema: "public",
                table: "parks",
                newName: "IX_parks_UserId");

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
    }
}
