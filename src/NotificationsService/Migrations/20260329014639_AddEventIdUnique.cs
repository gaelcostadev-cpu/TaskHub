using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsService.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_EventId",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_EventId",
                table: "notifications",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId_IsRead",
                table: "notifications",
                columns: new[] { "UserId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_EventId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_UserId_IsRead",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_EventId",
                table: "notifications",
                column: "EventId");
        }
    }
}
