using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Activities");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Activities",
                type: "time(0)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Activities",
                type: "time(0)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                column: "Day",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Activities");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Activities",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(0)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Activities",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(0)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Activities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
