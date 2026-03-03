using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksService.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_TaskId",
                table: "TaskAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_CreatedAt",
                table: "tasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_DueDate",
                table: "tasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_Priority",
                table: "tasks",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_Status",
                table: "tasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_AssignedUserId",
                table: "TaskAssignments",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_TaskId_AssignedUserId",
                table: "TaskAssignments",
                columns: new[] { "TaskId", "AssignedUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tasks_CreatedAt",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_DueDate",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_Priority",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_Status",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_AssignedUserId",
                table: "TaskAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_TaskId_AssignedUserId",
                table: "TaskAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_TaskId",
                table: "TaskAssignments",
                column: "TaskId");
        }
    }
}
