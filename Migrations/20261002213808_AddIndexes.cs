using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_IsOpen_CreatedAt",
                table: "Jobs",
                columns: new[] { "IsOpen", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobType",
                table: "Jobs",
                column: "JobType");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Location",
                table: "Jobs",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_Status",
                table: "JobApplications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_IsOpen_CreatedAt",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobType",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_Location",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_Status",
                table: "JobApplications");
        }
    }
}
