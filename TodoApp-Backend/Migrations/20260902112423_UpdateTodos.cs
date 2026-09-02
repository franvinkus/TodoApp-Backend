using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTodos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TodoPriority",
                table: "Todos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TodoPriority",
                table: "Todos");
        }
    }
}
