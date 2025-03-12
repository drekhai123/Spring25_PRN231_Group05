using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateFarmToolOfTask_Add_FarmToolsOfTaskUnit_And_FarmToolsOfTaskQuantity_IsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FarmToolsOfTaskUnit",
                table: "FarmToolsOfTasks",
                newName: "FarmToolOfTaskUnit");

            migrationBuilder.RenameColumn(
                name: "FarmToolsOfTaskQuantity",
                table: "FarmToolsOfTasks",
                newName: "FarmToolOfTaskQuantity");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FarmToolsOfTasks",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FarmToolsOfTasks");

            migrationBuilder.RenameColumn(
                name: "FarmToolOfTaskUnit",
                table: "FarmToolsOfTasks",
                newName: "FarmToolsOfTaskUnit");

            migrationBuilder.RenameColumn(
                name: "FarmToolOfTaskQuantity",
                table: "FarmToolsOfTasks",
                newName: "FarmToolsOfTaskQuantity");
        }
    }
}
