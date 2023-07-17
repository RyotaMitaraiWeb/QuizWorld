using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizWorld.Infrastructure.Migrations
{
    public partial class quizentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("05e139e1-d6cc-4b63-ae4a-abeadbf2ddb6"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5768a1ac-2d2c-4507-9aac-7f57eb7cc384"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d53a3c78-d770-4199-b07c-a3e4ce4fec6e"));

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The ID of the type")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false, comment: "The type of the question. 0 = single-choice, 1 = multiple-choice, 2 = text"),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "The short name of the type, which the client will typically use. The current short names are \"single\", \"multi\", and \"text\""),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "The full name of the type")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.Id);
                },
                comment: "The type of the question. Questions can be single-choice, multiple-choice, or text-based");

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The ID of the quiz")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "The title of the quiz"),
                    NormalizedTitle = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "The title of the quiz with all letters uppercased and spaces removed. Used for searching and sorting"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false, comment: "The version of the quiz. The version starts at 1 and increments every time the quiz is updated. When the quiz is retrieved, only questions that match the quiz's version will be included"),
                    InstantMode = table.Column<bool>(type: "bit", nullable: false, comment: "In instant mode, the user can check if their answer is correct as soon as they give one. If not in instant mode, the user has to answer all questions before grading them"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date on which the quiz was created."),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date on which the quiz was last updated. Equal to CreatedOn if it has never been updated."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "If marked as deleted, the quiz will not be retrieved at all"),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the user that created the quiz")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A quiz that has many questions, which on their own can have many answers");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the question"),
                    Prompt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "The content of the question itself (e.g. \"What is the capital of Mongolia?\""),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false, comment: "The ID of the type of this question."),
                    Version = table.Column<int>(type: "int", nullable: false, comment: "The version of the question. A question will only be retrieved if it matches the quiz's version or specified explicitely"),
                    QuizId = table.Column<int>(type: "int", nullable: false, comment: "The ID of the quiz which this question belongs to"),
                    Order = table.Column<int>(type: "int", nullable: false, comment: "The index of the question for the given quiz, used to reliably preserve the order that the creator wants")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A question in the given quiz.");

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary key for the answer. For single- and multiple-choice questions, the ID is also used on the client to determine if the user has answered the question correctly"),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly"),
                    Correct = table.Column<bool>(type: "bit", nullable: false, comment: "A mark that indicates whether the answer is considered correct or wrong. Text questions should have only correct answers."),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the question which this answer belongs to")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "An answer for the given question, which can be correct or wrong. For text questions, all answers should be correct");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("9ba9bb21-c16e-4351-88ab-0ce14f94d52a"), "cc7e1d43-a7df-4129-9b32-b472e8e77809", "Moderator", "MODERATOR" },
                    { new Guid("9e6b5887-25ce-464f-b5d6-9f2a8644d980"), "ed03eb66-3854-431e-8e38-c92ff7852975", "User", "USER" },
                    { new Guid("ed99a930-9c6d-4f2a-abde-eb30f15ec959"), "32b3baef-502c-4872-bb2d-7d479a11c65a", "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "Id", "FullName", "ShortName", "Type" },
                values: new object[,]
                {
                    { 1, "Single-choice", "single", 0 },
                    { 2, "Multiple-choice", "multi", 1 },
                    { 3, "Text", "text", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionTypeId",
                table: "Questions",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatorId",
                table: "Quizzes",
                column: "CreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9ba9bb21-c16e-4351-88ab-0ce14f94d52a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9e6b5887-25ce-464f-b5d6-9f2a8644d980"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ed99a930-9c6d-4f2a-abde-eb30f15ec959"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("05e139e1-d6cc-4b63-ae4a-abeadbf2ddb6"), "19e0fc70-dde3-49e7-a7a1-fd5c278aba32", "Moderator", "MODERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("5768a1ac-2d2c-4507-9aac-7f57eb7cc384"), "95aa57c9-dc04-4226-ba58-4a70a6058338", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("d53a3c78-d770-4199-b07c-a3e4ce4fec6e"), "94af65b2-fc39-4410-bceb-3419fac21881", "Administrator", "ADMINISTRATOR" });
        }
    }
}
