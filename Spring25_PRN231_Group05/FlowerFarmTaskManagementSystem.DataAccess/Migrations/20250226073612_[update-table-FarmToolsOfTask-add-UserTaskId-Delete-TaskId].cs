using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updatetableFarmToolsOfTaskaddUserTaskIdDeleteTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmToolsOfTasks_TaskWorks_TaskWorkId",
                table: "FarmToolsOfTasks");

            migrationBuilder.RenameColumn(
                name: "TaskWorkId",
                table: "FarmToolsOfTasks",
                newName: "UserTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmToolsOfTasks_TaskWorkId",
                table: "FarmToolsOfTasks",
                newName: "IX_FarmToolsOfTasks_UserTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmToolsOfTasks_UserTasks_UserTaskId",
                table: "FarmToolsOfTasks",
                column: "UserTaskId",
                principalTable: "UserTasks",
                principalColumn: "UserTaskId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmToolsOfTasks_UserTasks_UserTaskId",
                table: "FarmToolsOfTasks");

            migrationBuilder.RenameColumn(
                name: "UserTaskId",
                table: "FarmToolsOfTasks",
                newName: "TaskWorkId");

            migrationBuilder.RenameIndex(
                name: "IX_FarmToolsOfTasks_UserTaskId",
                table: "FarmToolsOfTasks",
                newName: "IX_FarmToolsOfTasks_TaskWorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmToolsOfTasks_TaskWorks_TaskWorkId",
                table: "FarmToolsOfTasks",
                column: "TaskWorkId",
                principalTable: "TaskWorks",
                principalColumn: "TaskWorkId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
