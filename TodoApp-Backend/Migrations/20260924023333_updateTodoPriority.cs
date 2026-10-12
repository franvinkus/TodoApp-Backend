using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp_Backend.Migrations
{
    /// <inheritdoc />
    public partial class updateTodoPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<int>(
            //    name: "TodoPriority",
            //    table: "Todos",
            //    type: "integer",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "text");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Todos""
                ALTER COLUMN ""TodoPriority"" DROP DEFAULT;

                ALTER TABLE ""Todos""
                ALTER COLUMN ""TodoPriority"" TYPE Integer
                USING CASE 
                    WHEN ""TodoPriority"" ='Low' THEN 1
                    WHEN ""TodoPriority"" ='Medium' THEN 2
                    WHEN ""TodoPriority"" ='High' THEN 3
                    ELSE 1 -- Default to Low if the value is unrecognized
                END;"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TodoPriority",
                table: "Todos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
