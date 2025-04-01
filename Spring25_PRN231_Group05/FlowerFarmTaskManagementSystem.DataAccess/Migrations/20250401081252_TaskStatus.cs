using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TaskStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskStatus",
                table: "TaskWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskStatus",
                table: "TaskWorks");
        }
    }
}
