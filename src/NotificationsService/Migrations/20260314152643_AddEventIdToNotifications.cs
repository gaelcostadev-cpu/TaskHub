using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsService.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_notifications",
                table: "notifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId",
                table: "notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_notifications",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_EventId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_UserId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "notifications");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");
        }
    }
}
