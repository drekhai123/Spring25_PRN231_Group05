using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateUserTask_FarmToolOfTask_ProductField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductivityUnit",
                table: "ProductFields",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Productivity",
                table: "ProductFields",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "FarmToolsOfTasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "FarmToolsOfTasks");

            migrationBuilder.UpdateData(
                table: "ProductFields",
                keyColumn: "ProductivityUnit",
                keyValue: null,
                column: "ProductivityUnit",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProductivityUnit",
                table: "ProductFields",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Productivity",
                table: "ProductFields",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);
        }
    }
}
