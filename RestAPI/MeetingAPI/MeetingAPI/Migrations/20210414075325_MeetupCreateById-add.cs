using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetingAPI.Migrations
{
    public partial class MeetupCreateByIdadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreateById",
                table: "Meetups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetups_CreateById",
                table: "Meetups",
                column: "CreateById");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetups_Users_CreateById",
                table: "Meetups",
                column: "CreateById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetups_Users_CreateById",
                table: "Meetups");

            migrationBuilder.DropIndex(
                name: "IX_Meetups_CreateById",
                table: "Meetups");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "Meetups");
        }
    }
}
