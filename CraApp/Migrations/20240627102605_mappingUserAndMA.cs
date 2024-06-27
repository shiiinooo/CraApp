using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraApp.Migrations
{
    /// <inheritdoc />
    public partial class mappingUserAndMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MonthlyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyActivities_UserId",
                table: "MonthlyActivities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyActivities_Users_UserId",
                table: "MonthlyActivities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyActivities_Users_UserId",
                table: "MonthlyActivities");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyActivities_UserId",
                table: "MonthlyActivities");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MonthlyActivities");
        }
    }
}
