using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatRoom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "ChatMessages",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CreatedByUserId",
                table: "ChatMessages",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_CreatedByUserId",
                table: "ChatMessages",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_CreatedByUserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_CreatedByUserId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450);
        }
    }
}
