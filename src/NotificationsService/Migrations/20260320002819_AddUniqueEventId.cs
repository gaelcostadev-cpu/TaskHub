using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsService.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEventId : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_EventId",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_EventId",
                table: "notifications",
                column: "EventId");
        }
    }
}
