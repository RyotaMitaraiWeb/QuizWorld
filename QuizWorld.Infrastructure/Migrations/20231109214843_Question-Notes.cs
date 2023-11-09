using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizWorld.Infrastructure.Migrations
{
    public partial class QuestionNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Questions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "The notes are displayed after the player answers a question. They can be used to clarify something (e.g. why a certain answer is correct/incorrect), provide relevant links, or simplyprovide more information that may be interesting to the user");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Answers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Answers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly");
        }
    }
}
