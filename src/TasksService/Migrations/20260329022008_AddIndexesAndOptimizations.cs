using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksService.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tasks_CreatedByUserId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_task_comments_TaskId",
                table: "task_comments");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_CreatedByUserId_CreatedAt",
                table: "tasks",
                columns: new[] { "CreatedByUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_task_comments_TaskId_CreatedAt",
                table: "task_comments",
                columns: new[] { "TaskId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tasks_CreatedByUserId_CreatedAt",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_task_comments_TaskId_CreatedAt",
                table: "task_comments");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_CreatedByUserId",
                table: "tasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_task_comments_TaskId",
                table: "task_comments",
                column: "TaskId");
        }
    }
}
