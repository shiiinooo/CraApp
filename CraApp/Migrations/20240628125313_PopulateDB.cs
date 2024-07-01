using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CraApp.Migrations
{
    /// <inheritdoc />
    public partial class PopulateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(0)", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Project = table.Column<int>(type: "int", nullable: false),
                    MonthlyActivitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_MonthlyActivities_MonthlyActivitiesId",
                        column: x => x.MonthlyActivitiesId,
                        principalTable: "MonthlyActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Password", "Role", "UserName" },
                values: new object[,]
                {
                    { 1, "Ahmed", "Password123#", "admin", "shiinoo" },
                    { 2, "Marouane", "Password123#", "admin", "PipInstallGeek" }
                });

            migrationBuilder.InsertData(
                table: "MonthlyActivities",
                columns: new[] { "Id", "Month", "UserId", "Year" },
                values: new object[,]
                {
                    { 1, 1, 1, 2024 },
                    { 2, 2, 1, 2024 }
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "Day", "EndTime", "MonthlyActivitiesId", "Project", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, new TimeSpan(0, 16, 0, 0, 0), 1, 0, new TimeSpan(0, 10, 0, 0, 0) },
                    { 3, 3, new TimeSpan(0, 16, 0, 0, 0), 2, 0, new TimeSpan(0, 10, 0, 0, 0) },
                    { 21, 2, new TimeSpan(0, 16, 0, 0, 0), 1, 1, new TimeSpan(0, 10, 0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_MonthlyActivitiesId",
                table: "Activities",
                column: "MonthlyActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyActivities_UserId",
                table: "MonthlyActivities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "MonthlyActivities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
