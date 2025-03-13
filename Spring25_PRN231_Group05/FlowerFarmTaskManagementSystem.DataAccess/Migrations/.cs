using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateFarmToolOfTask_Add_FarmToolsOfTaskUnit_FarmToolsOfTaskQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "TaskWorks");

            migrationBuilder.AddColumn<int>(
                name: "FarmToolsOfTaskQuantity",
                table: "FarmToolsOfTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FarmToolsOfTaskUnit",
                table: "FarmToolsOfTasks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FarmToolsOfTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmToolsOfTaskQuantity",
                table: "FarmToolsOfTasks");

            migrationBuilder.DropColumn(
                name: "FarmToolsOfTaskUnit",
                table: "FarmToolsOfTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FarmToolsOfTasks");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "TaskWorks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
