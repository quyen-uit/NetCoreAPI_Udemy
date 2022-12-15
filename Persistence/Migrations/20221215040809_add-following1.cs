using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addfollowing1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersFollowing_AspNetUsers_ObserverId",
                table: "UsersFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersFollowing_AspNetUsers_TargetId",
                table: "UsersFollowing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersFollowing",
                table: "UsersFollowing");

            migrationBuilder.RenameTable(
                name: "UsersFollowing",
                newName: "UserFollowings");

            migrationBuilder.RenameIndex(
                name: "IX_UsersFollowing_TargetId",
                table: "UserFollowings",
                newName: "IX_UserFollowings_TargetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollowings",
                table: "UserFollowings",
                columns: new[] { "ObserverId", "TargetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_AspNetUsers_ObserverId",
                table: "UserFollowings",
                column: "ObserverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_AspNetUsers_TargetId",
                table: "UserFollowings",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_AspNetUsers_ObserverId",
                table: "UserFollowings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_AspNetUsers_TargetId",
                table: "UserFollowings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollowings",
                table: "UserFollowings");

            migrationBuilder.RenameTable(
                name: "UserFollowings",
                newName: "UsersFollowing");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowings_TargetId",
                table: "UsersFollowing",
                newName: "IX_UsersFollowing_TargetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersFollowing",
                table: "UsersFollowing",
                columns: new[] { "ObserverId", "TargetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsersFollowing_AspNetUsers_ObserverId",
                table: "UsersFollowing",
                column: "ObserverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersFollowing_AspNetUsers_TargetId",
                table: "UsersFollowing",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
