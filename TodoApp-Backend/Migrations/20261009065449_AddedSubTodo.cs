using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sub_Todo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TodoId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Todo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sub_Todo_Todos_TodoId",
                        column: x => x.TodoId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Todo_TodoId",
                table: "Sub_Todo",
                column: "TodoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sub_Todo");
        }
    }
}
