using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraApp.Migrations
{
    /// <inheritdoc />
    public partial class AddingMonthlyActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyAID",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MonthlyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyActivities", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                column: "MonthlyAID",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_MonthlyAID",
                table: "Activities",
                column: "MonthlyAID");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyAID",
                table: "Activities",
                column: "MonthlyAID",
                principalTable: "MonthlyActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_MonthlyActivities_MonthlyAID",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "MonthlyActivities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_MonthlyAID",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "MonthlyAID",
                table: "Activities");
        }
    }
}
