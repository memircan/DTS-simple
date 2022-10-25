using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GidenEvraks_AspNetUsers_UserId",
                table: "GidenEvraks");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "GidenEvraks");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "GidenEvraks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GidenGonderilen",
                table: "GidenEvraks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_GidenEvraks_AspNetUsers_UserId",
                table: "GidenEvraks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GidenEvraks_AspNetUsers_UserId",
                table: "GidenEvraks");

            migrationBuilder.DropColumn(
                name: "GidenGonderilen",
                table: "GidenEvraks");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "GidenEvraks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "GidenEvraks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_GidenEvraks_AspNetUsers_UserId",
                table: "GidenEvraks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
