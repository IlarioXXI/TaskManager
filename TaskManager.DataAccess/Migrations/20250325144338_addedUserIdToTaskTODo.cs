using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedUserIdToTaskTODo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "ToDos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ToDos");
        }
    }
}
