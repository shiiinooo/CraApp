using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraApp.Migrations
{
    /// <inheritdoc />
    public partial class modifyingFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyAID",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "MonthlyAID",
                table: "Activities",
                newName: "MonthlyActivitiesId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_MonthlyAID",
                table: "Activities",
                newName: "IX_Activities_MonthlyActivitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyActivitiesId",
                table: "Activities",
                column: "MonthlyActivitiesId",
                principalTable: "MonthlyActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyActivitiesId",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "MonthlyActivitiesId",
                table: "Activities",
                newName: "MonthlyAID");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_MonthlyActivitiesId",
                table: "Activities",
                newName: "IX_Activities_MonthlyAID");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyAID",
                table: "Activities",
                column: "MonthlyAID",
                principalTable: "MonthlyActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
